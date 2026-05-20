using DungeonCrawler.Combat;
using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Syncs legacy PlayerHealth damage with HealthComponent.
    /// Attach alongside both on the player prefab.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class PlayerHealthBridge : MonoBehaviour
    {
        private PlayerHealth legacy;
        private HealthComponent health;

        private void Awake()
        {
            legacy = GetComponent<PlayerHealth>();
            health = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            if (legacy != null && health != null)
            {
                legacy.health = health.CurrentHealth;
                legacy.maxHealth = health.MaxHealth;
            }
        }

        // Call from enemy attacks: GetComponent<HealthComponent>().TakeDamage(x)
    }
}
