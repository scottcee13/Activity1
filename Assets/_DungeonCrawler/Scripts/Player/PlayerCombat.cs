using System.Collections;
using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private float meleeRange = 2.5f;
        [SerializeField] private float meleeOverlapRadius = 2.2f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private GameObject hitVfxPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private string attackTriggerName = "primaryAttack";
        [SerializeField] private string secondaryAttackTriggerName = "secondaryAttack";
        [SerializeField] private WeaponHitbox weaponHitbox;
        [SerializeField] private bool useSphereCastFallback = true;
        [SerializeField] private float knockbackForce = 12f;
        [SerializeField] private int unarmedDamage = 8;
        [SerializeField] private AudioClip defaultSwingSfx;
        [SerializeField] private AudioClip defaultHitSfx;
        [SerializeField] private float hitFrameFallbackDelay = 0.4f;
        [SerializeField] private string[] attackStateNames = { "PrimaryAttack", "SecondaryAttack" };

        private WeaponData equippedWeapon;
        private WeaponHitbox activeHitbox;
        private PlayerMovementLock movementLock;
        private float lastAttackTime;
        private bool attackInProgress;
        private bool hitRegisteredThisSwing;
        private Coroutine hitFallbackRoutine;

        public bool IsAttacking => attackInProgress;
        public bool HasWeaponEquipped => equippedWeapon != null;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            if (weaponHitbox == null)
                weaponHitbox = GetComponentInChildren<WeaponHitbox>(true);

            activeHitbox = weaponHitbox;
            ResolveEnemyLayers();

            movementLock = GetComponent<PlayerMovementLock>();
            if (movementLock == null)
                movementLock = gameObject.AddComponent<PlayerMovementLock>();
        }

        private void ResolveEnemyLayers()
        {
            if (enemyLayers.value != 0) return;

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            if (enemyLayer >= 0)
                enemyLayers = 1 << enemyLayer;
            else
                enemyLayers = ~0;
        }

        private void OnEnable()
        {
            GameEvents.OnWeaponEquipped += HandleWeaponEquipped;
            GameEvents.OnWeaponUnequipped += HandleWeaponUnequipped;
            CharacterControllerMovement.OnAttack += TryMeleeAttack;
        }

        private void OnDisable()
        {
            GameEvents.OnWeaponEquipped -= HandleWeaponEquipped;
            GameEvents.OnWeaponUnequipped -= HandleWeaponUnequipped;
            CharacterControllerMovement.OnAttack -= TryMeleeAttack;
            EndAttackImmediate();
        }

        private void Update()
        {
            if (attackInProgress)
                PollAnimatorAttackEnd();

            if (movementLock != null && movementLock.IsLocked) return;

            if (Input.GetMouseButtonDown(0))
                TryMeleeAttack();

            if (Input.GetMouseButtonDown(1))
                TrySecondaryAttack();
        }

        public void BindEquippedWeapon(EquippedWeapon instance, WeaponData data)
        {
            equippedWeapon = data;
            activeHitbox = instance != null && instance.Hitbox != null ? instance.Hitbox : null;
            ApplyWeaponStatsToHitbox();
        }

        public void ClearEquippedWeapon()
        {
            equippedWeapon = null;
            activeHitbox = null;
            weaponHitbox?.EndSwing();
        }

        private void HandleWeaponEquipped(WeaponData weapon)
        {
            equippedWeapon = weapon;
        }

        private void HandleWeaponUnequipped()
        {
            ClearEquippedWeapon();
        }

        private void ApplyWeaponStatsToHitbox()
        {
            if (activeHitbox == null || equippedWeapon == null) return;

            activeHitbox.SetDamage(equippedWeapon.weaponDamage);
            activeHitbox.SetKnockback(equippedWeapon.knockbackForce);
            activeHitbox.SetApplyDamage(!useSphereCastFallback);
            if (equippedWeapon.hitSfx != null)
                activeHitbox.SetHitSfx(equippedWeapon.hitSfx);
        }

        public bool TryMeleeAttack()
        {
            if (attackInProgress) return false;
            return BeginAttack(attackTriggerName);
        }

        public bool TrySecondaryAttack()
        {
            if (attackInProgress) return false;
            return BeginAttack(secondaryAttackTriggerName);
        }

        private bool BeginAttack(string trigger)
        {
            float cooldown = equippedWeapon != null ? equippedWeapon.attackCooldown : 0.45f;
            if (Time.time < lastAttackTime + cooldown) return false;

            lastAttackTime = Time.time;
            attackInProgress = true;
            hitRegisteredThisSwing = false;

            movementLock.ForceUnlock();
            movementLock.LockMovement();

            if (animator != null && !string.IsNullOrEmpty(trigger))
            {
                animator.ResetTrigger(trigger);
                animator.SetTrigger(trigger);
            }

            AudioClip swing = equippedWeapon != null && equippedWeapon.swingSfx != null
                ? equippedWeapon.swingSfx
                : defaultSwingSfx;

            if (swing != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(swing);

            if (hitFallbackRoutine != null)
                StopCoroutine(hitFallbackRoutine);

            hitFallbackRoutine = StartCoroutine(HitFrameFallbackOnly());

            GameEvents.RaiseAbilityUsed("melee");
            return true;
        }

        private IEnumerator HitFrameFallbackOnly()
        {
            yield return new WaitForSeconds(hitFrameFallbackDelay);
            if (attackInProgress && !hitRegisteredThisSwing)
                OnAttackHitFrame();
        }

        private void PollAnimatorAttackEnd()
        {
            if (animator == null) return;

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (animator.IsInTransition(0)) return;

            bool inAttackState = false;
            foreach (string stateName in attackStateNames)
            {
                if (!state.IsName(stateName)) continue;

                inAttackState = true;
                if (state.normalizedTime >= 0.95f)
                    OnAttackAnimationEnd();
                break;
            }

            if (!inAttackState)
                OnAttackAnimationEnd();
        }

        public void OnAttackHitFrame()
        {
            if (!attackInProgress || hitRegisteredThisSwing) return;

            int damage = equippedWeapon != null ? equippedWeapon.weaponDamage : unarmedDamage;
            float kb = equippedWeapon != null ? equippedWeapon.knockbackForce : knockbackForce;

            if (equippedWeapon != null && activeHitbox != null)
            {
                activeHitbox.SetDamage(damage);
                activeHitbox.SetKnockback(kb);
                activeHitbox.BeginSwing();
            }

            if (useSphereCastFallback)
                OverlapMeleeAttack(damage, kb);

            hitRegisteredThisSwing = true;

            AudioClip hit = equippedWeapon != null && equippedWeapon.hitSfx != null
                ? equippedWeapon.hitSfx
                : defaultHitSfx;

            if (hit != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(hit);
        }

        public void OnAttackAnimationEnd()
        {
            if (!attackInProgress) return;

            activeHitbox?.EndSwing();
            weaponHitbox?.EndSwing();
            attackInProgress = false;
            hitRegisteredThisSwing = false;
            movementLock.ForceUnlock();

            if (hitFallbackRoutine != null)
            {
                StopCoroutine(hitFallbackRoutine);
                hitFallbackRoutine = null;
            }
        }

        private void EndAttackImmediate()
        {
            activeHitbox?.EndSwing();
            weaponHitbox?.EndSwing();
            attackInProgress = false;
            hitRegisteredThisSwing = false;
            movementLock?.ForceUnlock();

            if (hitFallbackRoutine != null)
            {
                StopCoroutine(hitFallbackRoutine);
                hitFallbackRoutine = null;
            }
        }

        private void OverlapMeleeAttack(int damage, float kb)
        {
            Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position + Vector3.up * 1f;
            Vector3 dir = transform.forward;
            if (PlayerAimProvider.Instance != null)
            {
                dir = PlayerAimProvider.Instance.AimForward;
                if (dir.sqrMagnitude < 0.01f)
                    dir = transform.forward;
            }

            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f)
                dir = transform.forward;
            dir.Normalize();

            float radius = Mathf.Max(meleeOverlapRadius, meleeRange);
            Vector3 center = origin + dir * (radius * 0.45f);
            Collider[] hits = Physics.OverlapSphere(center, radius, enemyLayers, QueryTriggerInteraction.Collide);

            HealthComponent best = null;
            float bestDist = float.MaxValue;
            Vector3 hitPoint = center;

            foreach (Collider col in hits)
            {
                HealthComponent health = col.GetComponentInParent<HealthComponent>();
                if (health == null || health.IsDead) continue;

                float dist = (health.transform.position - origin).sqrMagnitude;
                if (dist >= bestDist) continue;

                bestDist = dist;
                best = health;
                hitPoint = col.ClosestPoint(center);
            }

            if (best == null) return;

            Vector3 knockDir = best.transform.position - transform.position;
            knockDir.y = 0f;
            best.TakeDamage(damage, knockDir, kb, true);

            if (hitVfxPrefab != null)
                Instantiate(hitVfxPrefab, hitPoint, Quaternion.identity);
        }
    }
}
