using DungeonCrawler.Combat;
using DungeonCrawler.UI;
using UnityEngine;

namespace DungeonCrawler.Boss
{
    /// <summary>
    /// Activates boss and boss health bar when player enters arena.
    /// </summary>
    public class BossArenaTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject bossRoot;
        [SerializeField] private BossHealthBarUI bossHealthBar;
        [SerializeField] private GameObject arenaGate;
        [SerializeField] private bool oneShot = true;

        private bool triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (oneShot && triggered) return;

            triggered = true;

            if (bossRoot != null)
            {
                bossRoot.SetActive(true);
                BossController boss = bossRoot.GetComponent<BossController>();
                if (boss != null)
                    boss.ActivateBoss();
            }

            if (bossHealthBar != null && bossRoot != null)
            {
                HealthComponent health = bossRoot.GetComponent<HealthComponent>();
                if (health != null)
                    bossHealthBar.BindBoss(health);
            }

            if (arenaGate != null)
                arenaGate.SetActive(true);
        }
    }
}
