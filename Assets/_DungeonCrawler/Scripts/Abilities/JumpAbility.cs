using UnityEngine;

namespace DungeonCrawler.Abilities
{
    /// <summary>
    /// Jump using CharacterController. Integrates with PlayerMotor gravity.
    /// </summary>
    public class JumpAbility : AbilityBase
    {
        [SerializeField] private float jumpForce = 7f;
        private Player.PlayerMotor motor;

        private void Awake()
        {
            motor = GetComponent<Player.PlayerMotor>();
        }

        protected override bool CanExecute()
        {
            return motor != null && motor.IsGrounded;
        }

        protected override void Execute()
        {
            motor.ApplyJump(jumpForce);
        }
    }
}
