using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Player
{
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerDeathHandler : MonoBehaviour
    {
        private HealthComponent health;
        private bool processed;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            if (processed) return;
            processed = true;

            PlayerMotor motor = GetComponent<PlayerMotor>();
            if (motor != null) motor.enabled = false;

            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null) combat.enabled = false;

            WeaponEquipManager equip = GetComponent<WeaponEquipManager>();
            if (equip != null) equip.enabled = false;

            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            foreach (Collider col in GetComponentsInChildren<Collider>())
                col.enabled = false;

            PlayerMovementLock movementLock = GetComponent<PlayerMovementLock>();
            movementLock?.ForceUnlock();

            if (health != null && health.IsDead)
            {
                GameEvents.RaisePlayerDied();
                PlayerHealth.OnPlayerDeath?.Invoke();
            }
        }
    }
}
