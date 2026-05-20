using UnityEngine;

namespace DungeonCrawler.Abilities
{
    /// <summary>
    /// Quick burst movement in facing direction. Used in parkour room (Room 3).
    /// </summary>
    public class DashAbility : AbilityBase
    {
        [SerializeField] private float dashDistance = 8f;
        [SerializeField] private float dashDuration = 0.2f;

        private CharacterController controller;
        private Vector3 dashVelocity;
        private float dashTimer;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        protected override bool CanExecute()
        {
            return controller != null && dashTimer <= 0f;
        }

        protected override void Execute()
        {
            Vector3 forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();
            dashVelocity = forward * (dashDistance / dashDuration);
            dashTimer = dashDuration;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(null);
        }

        private void LateUpdate()
        {
            if (dashTimer <= 0f || controller == null) return;

            controller.Move(dashVelocity * Time.deltaTime);
            dashTimer -= Time.deltaTime;
        }
    }
}
