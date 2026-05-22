using DungeonCrawler.Boss;
using DungeonCrawler.Player;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Animation event receiver. Add to Player, enemies, and boss; wire clip events to these methods.
    /// </summary>
    public class CombatAnimationEvents : MonoBehaviour
    {
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private EnemyAttack enemyAttack;
        [SerializeField] private BossController bossController;

        private void Awake()
        {
            if (playerCombat == null)
                playerCombat = GetComponent<PlayerCombat>();

            if (enemyAttack == null)
                enemyAttack = GetComponent<EnemyAttack>();

            if (bossController == null)
                bossController = GetComponent<BossController>();
        }

        public void OnAttackHit()
        {
            if (playerCombat != null)
                playerCombat.OnAttackHitFrame();

            if (enemyAttack != null)
                enemyAttack.OnAttackHit();

            if (bossController != null)
                bossController.OnAttackHit();
        }

        public void OnAttackEnd()
        {
            if (playerCombat != null)
                playerCombat.OnAttackAnimationEnd();

            if (enemyAttack != null)
                enemyAttack.OnAttackEnd();

            if (bossController != null)
                bossController.OnAttackEnd();
        }

        public void OnFootstep()
        {
        }
    }
}
