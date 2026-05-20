using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Bridges legacy EnemyHealth to HealthComponent for the new combat pipeline.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyHealthBridge : MonoBehaviour
    {
        private EnemyHealth legacy;
        private HealthComponent health;

        private void Awake()
        {
            legacy = GetComponent<EnemyHealth>();
            health = GetComponent<HealthComponent>();
        }

        private void Update()
        {
            if (legacy == null || health == null) return;
            legacy.currentHP = health.CurrentHealth;
        }

        public void SyncFromLegacy()
        {
            if (legacy != null && health != null)
                health.TakeDamage(legacy.maxHP - legacy.currentHP);
        }
    }
}
