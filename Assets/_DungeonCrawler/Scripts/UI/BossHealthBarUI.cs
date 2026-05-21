using DungeonCrawler.Combat;
using UnityEngine;

namespace DungeonCrawler.UI
{
    public class BossHealthBarUI : HealthBarUI
    {
        [SerializeField] private string bossEntityId = "dungeon_boss";
        [SerializeField] private bool findBossOnStart = true;
        [SerializeField] private GameObject barRoot;

        protected override void Start()
        {
            if (barRoot != null)
                barRoot.SetActive(false);

            if (findBossOnStart)
            {
                HealthComponent[] all = FindObjectsByType<HealthComponent>(FindObjectsSortMode.None);
                foreach (HealthComponent h in all)
                {
                    if (h.IsDead || h.EntityId != bossEntityId) continue;
                    BindBoss(h);
                    return;
                }
            }

            base.Start();
        }

        public void BindBoss(HealthComponent boss)
        {
            if (boss == null) return;

            if (barRoot != null)
                barRoot.SetActive(true);

            SetTarget(boss);
        }
    }
}
