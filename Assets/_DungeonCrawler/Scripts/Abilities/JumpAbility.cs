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
        private Player.PlayerMovementLock movementLock;

        private void Awake()
        {
            motor = GetComponent<Player.PlayerMotor>();
            movementLock = GetComponent<Player.PlayerMovementLock>();
        }

        protected override bool CanExecute()
        {
            if (movementLock != null && movementLock.IsLocked) return false;
            return motor != null && motor.IsGrounded;
        }

        protected override void Execute()
        {
            motor.ApplyJump(jumpForce);
        }
    }
}
