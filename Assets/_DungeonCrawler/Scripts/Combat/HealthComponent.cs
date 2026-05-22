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

        private bool deathHandled;

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
            TakeDamage(amount, Vector3.zero, 0f, false);
        }

        public void TakeDamage(int amount, Vector3 knockbackDirection, float knockbackForce)
        {
            TakeDamage(amount, knockbackDirection, knockbackForce, knockbackForce > 0f);
        }

        public void TakeDamage(int amount, Vector3 knockbackDirection, float knockbackForce, bool applyKnockback)
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

            if (applyKnockback)
                ApplyKnockback(knockbackDirection, knockbackForce);

            if (isPlayer)
                HandlePlayerDamaged();

            if (CurrentHealth <= 0)
                Die();
        }

        private void HandlePlayerDamaged()
        {
            Animator playerAnimator = GetComponentInChildren<Animator>();
            if (playerAnimator != null)
                playerAnimator.SetTrigger("hurt");

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

        private void ApplyKnockback(Vector3 direction, float force)
        {
            if (force <= 0f || IsDead) return;

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
            if (deathHandled) return;
            deathHandled = true;

            OnDeath?.Invoke();

            if (isPlayer)
                return;

            GameEvents.RaiseEnemyKilled(entityId);

            EntityDeathHandler deathHandler = GetComponent<EntityDeathHandler>();
            if (deathHandler == null)
                deathHandler = gameObject.AddComponent<EntityDeathHandler>();

            deathHandler.ForceDeathCleanup();
        }
    }
}
