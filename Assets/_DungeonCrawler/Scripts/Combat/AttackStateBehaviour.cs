using DungeonCrawler.Player;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Optional Animator state behaviour on attack states — ends attack when state exits (no extra lock here).
    /// </summary>
    public class AttackStateBehaviour : StateMachineBehaviour
    {
        [SerializeField] private bool isPlayerAttack;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!isPlayerAttack) return;

            PlayerCombat combat = animator.GetComponentInParent<PlayerCombat>();
            if (combat != null)
                combat.OnAttackAnimationEnd();
        }
    }
}
