using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private float meleeRange = 2.5f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private GameObject hitVfxPrefab;
        [SerializeField] private Animator animator;
        [SerializeField] private string attackTriggerName = "primaryAttack";
        [SerializeField] private WeaponHitbox weaponHitbox;
        [SerializeField] private float hitboxActiveTime = 0.35f;
        [SerializeField] private AudioClip attackSfx;
        [SerializeField] private bool useSphereCastFallback = true;
        [SerializeField] private float knockbackForce = 12f;

        private WeaponData equippedWeapon;
        private float lastAttackTime;
        private float hitboxTimer;

        private void Awake()
        {
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            if (weaponHitbox == null)
                weaponHitbox = GetComponentInChildren<WeaponHitbox>();
        }

        private void OnEnable()
        {
            GameEvents.OnWeaponEquipped += HandleWeaponEquipped;
            CharacterControllerMovement.OnAttack += TryMeleeAttack;
        }

        private void OnDisable()
        {
            GameEvents.OnWeaponEquipped -= HandleWeaponEquipped;
            CharacterControllerMovement.OnAttack -= TryMeleeAttack;
        }

        private void Update()
        {
            if (hitboxTimer > 0f)
            {
                hitboxTimer -= Time.deltaTime;
                if (hitboxTimer <= 0f && weaponHitbox != null)
                    weaponHitbox.EndSwing();
            }

            if (Input.GetMouseButtonDown(0))
                TryMeleeAttack();
        }

        private void HandleWeaponEquipped(WeaponData weapon)
        {
            equippedWeapon = weapon;
            if (weaponHitbox != null && weapon != null)
                weaponHitbox.SetDamage(weapon.weaponDamage);
        }

        public bool TryMeleeAttack()
        {
            float cooldown = equippedWeapon != null ? equippedWeapon.attackCooldown : 0.5f;
            int damage = equippedWeapon != null ? equippedWeapon.weaponDamage : 10;

            if (Time.time < lastAttackTime + cooldown) return false;
            lastAttackTime = Time.time;

            if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
                animator.SetTrigger(attackTriggerName);

            if (weaponHitbox != null)
            {
                weaponHitbox.SetDamage(damage);
                weaponHitbox.BeginSwing();
                hitboxTimer = hitboxActiveTime;
            }
            else if (useSphereCastFallback)
            {
                SphereCastAttack(damage);
            }

            if (attackSfx != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(attackSfx);

            GameEvents.RaiseAbilityUsed("melee");
            return true;
        }

        private void SphereCastAttack(int damage)
        {
            Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position + Vector3.up;
            if (Physics.SphereCast(origin, 0.5f, transform.forward, out RaycastHit hit, meleeRange, enemyLayers))
            {
                HealthComponent health = hit.collider.GetComponentInParent<HealthComponent>();
                if (health != null)
                {
                    Vector3 dir = hit.collider.transform.position - transform.position;
                    dir.y = 0f;
                    health.TakeDamage(damage, dir, knockbackForce);
                }

                if (hitVfxPrefab != null)
                    Instantiate(hitVfxPrefab, hit.point, Quaternion.identity);
            }
        }

        /// <summary>Call from animation event at end of swing.</summary>
        public void OnAttackAnimationEnd()
        {
            if (weaponHitbox != null)
                weaponHitbox.EndSwing();
        }
    }
}
