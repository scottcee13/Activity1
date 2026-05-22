using UnityEngine;

namespace DungeonCrawler.Combat
{
    public static class CombatDamage
    {
        public static void Deal(Transform victim, int amount, Transform attacker, float knockbackForce = 5f)
        {
            if (victim == null || amount <= 0) return;

            HealthComponent health = victim.GetComponent<HealthComponent>();
            if (health == null)
                health = victim.GetComponentInParent<HealthComponent>();

            if (health != null && health.IsDead) return;

            Vector3 knockDir = Vector3.forward;
            if (attacker != null)
            {
                knockDir = victim.position - attacker.position;
                knockDir.y = 0f;
            }

            if (knockDir.sqrMagnitude < 0.01f)
                knockDir = victim.forward;

            knockDir.Normalize();

            if (health != null)
            {
                health.TakeDamage(amount, knockDir, knockbackForce, knockbackForce > 0f);
                return;
            }

            PlayerHealth legacy = victim.GetComponent<PlayerHealth>();
            if (legacy != null)
                legacy.ApplyDamage(amount);
        }
    }
}
