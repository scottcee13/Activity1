using DungeonCrawler.Combat;
using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Auto-adds required player gameplay components. Runs before other player scripts.
    /// </summary>
    [DefaultExecutionOrder(-250)]
    public class PlayerGameplayBootstrap : MonoBehaviour
    {
        [SerializeField] private bool startUnequipped = true;
        [SerializeField] private bool logSetup;

        private void Awake()
        {
            Ensure<PlayerMovementLock>();
            Ensure<PlayerCombat>();
            Ensure<WeaponEquipManager>();
            Ensure<PlayerWeapon>();
            Ensure<CombatAnimationEvents>();
            Ensure<PlayerDeathHandler>();
            Ensure<KnockbackReceiver>();

            WeaponEquipManager equip = GetComponent<WeaponEquipManager>();
            if (equip != null && startUnequipped)
                equip.ConfigureStartUnequipped();

            if (logSetup)
                Debug.Log("[PlayerGameplayBootstrap] Player combat components ready.");
        }

        private T Ensure<T>() where T : Component
        {
            T component = GetComponent<T>();
            if (component != null) return component;
            return gameObject.AddComponent<T>();
        }
    }
}
