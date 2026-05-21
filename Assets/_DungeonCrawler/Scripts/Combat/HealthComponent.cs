using System;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private string entityId;
        [SerializeField] private bool isPlayer;

        public string EntityId => entityId;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDeath;

        private void Awake()
        {
            if (CompareTag("Player"))
                isPlayer = true;
        }

        public void ConfigureEntityId(string id)
        {
            if (!string.IsNullOrEmpty(id))
                entityId = id;
        }

        private void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            TakeDamage(amount, Vector3.zero, 0f);
        }

        public void TakeDamage(int amount, Vector3 knockbackDirection, float knockbackForce)
        {
            if (IsDead || amount <= 0) return;

            if (isPlayer)
            {
                Player.PlayerDamageReceiver receiver = GetComponent<Player.PlayerDamageReceiver>();
                if (receiver != null && receiver.IsInvulnerable)
                    return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

            ApplyKnockback(knockbackDirection, knockbackForce);

            if (isPlayer)
            {
                GameEvents.RaisePlayerHealthChanged(CurrentHealth);
                PlayerHealth legacy = GetComponent<PlayerHealth>();
                if (legacy != null)
                {
                    legacy.SyncFromHealthComponent(CurrentHealth);
                    PlayerHealth.OnPlayerDamaged?.Invoke(CurrentHealth);
                }

                Player.PlayerDamageReceiver invuln = GetComponent<Player.PlayerDamageReceiver>();
                invuln?.ApplyInvulnerability();
            }

            if (CurrentHealth <= 0)
                Die();
        }

        private void ApplyKnockback(Vector3 direction, float force)
        {
            if (force <= 0f) return;

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f) return;

            KnockbackReceiver knockback = GetComponent<KnockbackReceiver>();
            if (knockback == null)
                knockback = gameObject.AddComponent<KnockbackReceiver>();

            knockback.ApplyKnockback(direction, force);
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

                EnemyFSM fsm = GetComponent<EnemyFSM>();
                if (fsm != null) fsm.enabled = false;

                UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                {
                    if (agent.enabled && agent.isOnNavMesh)
                        agent.isStopped = true;
                    agent.enabled = false;
                }
            }
        }
    }
}
