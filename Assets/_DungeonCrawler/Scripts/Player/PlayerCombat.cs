using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Melee and ranged combat. Connects weapons from InventoryManager to damage dealing.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private float meleeRange = 2.5f;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private GameObject hitVfxPrefab;

        private WeaponData equippedWeapon;
        private float lastAttackTime;

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

        private void HandleWeaponEquipped(WeaponData weapon)
        {
            equippedWeapon = weapon;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                TryMeleeAttack();
        }

        public bool TryMeleeAttack()
        {
            WeaponData weapon = equippedWeapon;
            if (weapon == null && PlayerWeaponExists())
            {
                // Fallback: legacy PlayerWeapon debug path
                return false;
            }

            float cooldown = weapon != null ? weapon.attackCooldown : 0.5f;
            int damage = weapon != null ? weapon.weaponDamage : 10;

            if (Time.time < lastAttackTime + cooldown) return false;
            lastAttackTime = Time.time;

            Vector3 origin = attackOrigin != null ? attackOrigin.position : transform.position + Vector3.up;
            if (Physics.SphereCast(origin, 0.5f, transform.forward, out RaycastHit hit, meleeRange, enemyLayers))
            {
                HealthComponent health = hit.collider.GetComponentInParent<HealthComponent>();
                if (health != null)
                    health.TakeDamage(damage);

                if (hitVfxPrefab != null)
                    Instantiate(hitVfxPrefab, hit.point, Quaternion.identity);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(null);
            }

            GameEvents.RaiseAbilityUsed("melee");
            return true;
        }

        private bool PlayerWeaponExists() => GetComponent<PlayerWeapon>() != null;
    }
}
