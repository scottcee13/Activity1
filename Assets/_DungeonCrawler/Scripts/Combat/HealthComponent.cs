using System;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Unified health for player, enemies, and boss. Fires events on damage/death.
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private string entityId;
        [SerializeField] private bool isPlayer;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            if (IsDead || amount <= 0) return;

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            if (isPlayer)
            {
                GameEvents.RaisePlayerHealthChanged(CurrentHealth);
                PlayerHealth legacy = GetComponent<PlayerHealth>();
                if (legacy != null)
                {
                    legacy.health = CurrentHealth;
                    PlayerHealth.OnPlayerDamaged?.Invoke(CurrentHealth);
                }
            }

            if (CurrentHealth <= 0)
                Die();
        }

        public void Heal(int amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();

            if (isPlayer)
            {
                GameEvents.RaisePlayerDied();
                PlayerHealth.OnPlayerDeath?.Invoke();
            }
            else
            {
                GameEvents.RaiseEnemyKilled(entityId);
            }

            if (!isPlayer)
            {
                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
}
