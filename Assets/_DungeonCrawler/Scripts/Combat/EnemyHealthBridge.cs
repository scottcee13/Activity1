using UnityEngine;

namespace DungeonCrawler.Combat
{
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

        private void Start()
        {
            if (legacy == null || health == null) return;

            legacy.maxHP = health.MaxHealth;
            legacy.currentHP = health.CurrentHealth;
            health.OnHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (health != null)
                health.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            if (legacy != null)
                legacy.currentHP = current;
        }
    }
}
