using UnityEngine;

namespace DungeonCrawler.Boss
{
    [CreateAssetMenu(fileName = "BossData", menuName = "Dungeon/Boss Data")]
    public class BossDataSO : ScriptableObject
    {
        public string bossId;
        public string displayName;
        public int maxHealth = 500;
        public float phase2HealthPercent = 0.5f;
        public float attackCooldown = 2f;
        public int meleeDamage = 20;
        public int rangedDamage = 15;
    }
}
