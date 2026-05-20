using DungeonCrawler.Combat;
using UnityEngine;

namespace DungeonCrawler.World
{
  /// <summary>
  /// Traps/lava — damage over time or on enter.
  /// </summary>
    public class HazardDamage : MonoBehaviour
    {
        [SerializeField] private int damagePerTick = 10;
        [SerializeField] private float tickInterval = 1f;

        private float timer;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            timer -= Time.deltaTime;
            if (timer > 0f) return;
            timer = tickInterval;

            HealthComponent health = other.GetComponent<HealthComponent>();
            health?.TakeDamage(damagePerTick);
        }
    }
}
