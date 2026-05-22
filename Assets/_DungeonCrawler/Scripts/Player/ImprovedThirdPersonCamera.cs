using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Smooth third-person orbit camera with collision offset. Room 3 parkour friendly.
    /// </summary>
    public class ImprovedThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform pivot;
        [SerializeField] private float distance = 5f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float sensitivity = 180f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float collisionBuffer = 0.2f;

        private float yaw;
        private float pitch = 15f;

        private void Start()
        {
            if (target == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) target = p.transform;
            }

            if (pivot != null && pivot.GetComponent<PlayerAimProvider>() == null)
                pivot.gameObject.AddComponent<PlayerAimProvider>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (target == null || pivot == null) return;
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.IsPaused) return;

            PlayerMovementLock movementLock = target != null ? target.GetComponent<PlayerMovementLock>() : null;
            PlayerCombat combat = target != null ? target.GetComponent<PlayerCombat>() : null;
            if (movementLock != null && movementLock.IsLocked) return;
            if (combat != null && combat.IsAttacking) return;

            float mx = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float my = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            yaw += mx;
            pitch -= my;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            pivot.position = target.position + Vector3.up * 1.6f;
            pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desired = pivot.position - pivot.forward * distance;
            float resolvedDistance = distance;

            if (Physics.SphereCast(
                pivot.position,
                0.2f,
                -pivot.forward,
                out RaycastHit hit,
                distance,
                collisionMask))
            {
                resolvedDistance = Mathf.Clamp(hit.distance - collisionBuffer, minDistance, distance);
                desired = pivot.position - pivot.forward * resolvedDistance;
            }

            transform.position = desired;
            transform.LookAt(pivot.position);
        }
    }
}
