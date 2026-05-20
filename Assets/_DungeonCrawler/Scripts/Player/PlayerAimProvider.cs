using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Stores horizontal aim direction from the camera pivot (updated in LateUpdate).
    /// Movement and projectiles read this so shots go where the player looks.
    /// </summary>
    public class PlayerAimProvider : MonoBehaviour
    {
        public static PlayerAimProvider Instance { get; private set; }

        /// <summary>Flat forward on XZ plane (where the camera looks).</summary>
        public Vector3 AimForward { get; private set; } = Vector3.forward;

        /// <summary>Full 3D forward including pitch (for projectiles).</summary>
        public Vector3 AimDirection3D { get; private set; } = Vector3.forward;

        public Quaternion AimRotation => Quaternion.LookRotation(AimForward, Vector3.up);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            UpdateAim();
        }

        private void LateUpdate()
        {
            UpdateAim();
        }

        private void UpdateAim()
        {
            Vector3 forward = transform.forward;
            AimDirection3D = forward.normalized;

            Vector3 flat = forward;
            flat.y = 0f;
            AimForward = flat.sqrMagnitude > 0.0001f ? flat.normalized : transform.forward;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
