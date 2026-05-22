using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Single authoritative movement driver. Runs in Update (not FixedUpdate) with CharacterController.
    /// Camera-relative direction comes from PlayerAimProvider on the camera pivot.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [DefaultExecutionOrder(100)]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 1.6f;
        [SerializeField] private float rotationSmoothing = 12f;
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float groundCheckDistance = 0.25f;
        [SerializeField] private Transform cameraPivot;

        private CharacterController controller;
        private Vector3 verticalVelocity;
        private Animator animator;
        private PlayerMovementLock movementLock;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            movementLock = GetComponent<PlayerMovementLock>();

            if (cameraPivot == null && PlayerAimProvider.Instance != null)
                cameraPivot = PlayerAimProvider.Instance.transform;
        }

        public void SetCameraPivot(Transform pivot)
        {
            cameraPivot = pivot;
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.IsPaused)
                return;

            UpdateGrounded();
            HandleGravity();

            if (movementLock != null && movementLock.IsLocked)
            {
                ApplyMovement(Vector3.zero);
                UpdateAnimator(Vector3.zero);
                return;
            }

            PlayerCombat combat = GetComponent<PlayerCombat>();
            if (combat != null && combat.IsAttacking)
            {
                ApplyMovement(Vector3.zero);
                UpdateAnimator(Vector3.zero);
                return;
            }

            Vector3 move = GetCameraRelativeMove();
            ApplyMovement(move);
            RotateTowardMove(move);
            UpdateAnimator(move);
        }

        private void UpdateGrounded()
        {
            IsGrounded = controller.isGrounded;
            if (!IsGrounded)
            {
                Vector3 origin = transform.position + Vector3.up * 0.15f;
                IsGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance + controller.skinWidth);
            }
        }

        private void HandleGravity()
        {
            if (IsGrounded && verticalVelocity.y < 0f)
                verticalVelocity.y = -2f;

            verticalVelocity.y += gravity * Time.deltaTime;
        }

        private Vector3 GetCameraRelativeMove()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(h, 0f, v);
            if (input.sqrMagnitude < 0.01f) return Vector3.zero;

            input.Normalize();

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;

            if (PlayerAimProvider.Instance != null)
            {
                forward = PlayerAimProvider.Instance.AimForward;
                right = Vector3.Cross(Vector3.up, forward).normalized;
            }
            else if (cameraPivot != null)
            {
                forward = cameraPivot.forward;
                forward.y = 0f;
                forward.Normalize();
                right = cameraPivot.right;
                right.y = 0f;
                right.Normalize();
            }

            return (forward * input.z + right * input.x).normalized;
        }

        private void ApplyMovement(Vector3 move)
        {
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftControl))
                speed *= sprintMultiplier;

            Vector3 velocity = move * speed + Vector3.up * verticalVelocity.y;
            controller.Move(velocity * Time.deltaTime);
        }

        private void RotateTowardMove(Vector3 move)
        {
            if (move.sqrMagnitude < 0.01f) return;

            Quaternion target = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                rotationSmoothing * Time.deltaTime
            );
        }

        private void UpdateAnimator(Vector3 move)
        {
            if (animator == null) return;

            bool walking = move.sqrMagnitude > 0.01f;
            bool running = walking && Input.GetKey(KeyCode.LeftControl);

            animator.SetBool("isWalking", walking && !running);
            animator.SetBool("isRunning", running);
        }

        public void ApplyJump(float force)
        {
            if (!IsGrounded) return;
            verticalVelocity.y = force;
        }

        public void ResetVerticalVelocity()
        {
            verticalVelocity.y = -2f;
        }
    }
}
