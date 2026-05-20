using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Fixes jitter: disables duplicate movement scripts and extra colliders on the Player.
    /// Attach once on the Player root in the dungeon scene.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class PlayerMovementBootstrap : MonoBehaviour
    {
        [Header("Disable these duplicate movers (cause jitter)")]
        [SerializeField] private bool disableLegacyMovement = true;
        [SerializeField] private bool disableExtraCapsuleCollider = true;
        [SerializeField] private bool ensurePlayerMotor = true;

        [Header("Camera pivot (child with PlayerAimProvider)")]
        [SerializeField] private Transform cameraPivot;

        private void Awake()
        {
            if (disableLegacyMovement)
            {
                CharacterControllerMovement legacyInput = GetComponent<CharacterControllerMovement>();
                if (legacyInput != null)
                {
                    legacyInput.enabled = false;
                    Debug.Log("[PlayerMovementBootstrap] Disabled CharacterControllerMovement (use PlayerMotor instead).");
                }

                ThirdPersonPlayerMovement legacyMove = GetComponent<ThirdPersonPlayerMovement>();
                if (legacyMove != null)
                {
                    legacyMove.enabled = false;
                    Debug.Log("[PlayerMovementBootstrap] Disabled ThirdPersonPlayerMovement (duplicate CharacterController.Move).");
                }
            }

            if (disableExtraCapsuleCollider)
            {
                CapsuleCollider capsule = GetComponent<CapsuleCollider>();
                CharacterController cc = GetComponent<CharacterController>();
                if (capsule != null && cc != null)
                {
                    capsule.enabled = false;
                    Debug.Log("[PlayerMovementBootstrap] Disabled CapsuleCollider — CharacterController handles collision.");
                }
            }

            if (ensurePlayerMotor && GetComponent<PlayerMotor>() == null)
            {
                gameObject.AddComponent<PlayerMotor>();
                Debug.Log("[PlayerMovementBootstrap] Added PlayerMotor.");
            }

            ResolveCameraPivot();
        }

        private void ResolveCameraPivot()
        {
            if (cameraPivot == null)
            {
                PlayerAimProvider aim = GetComponentInChildren<PlayerAimProvider>();
                if (aim != null) cameraPivot = aim.transform;
            }

            if (cameraPivot == null)
            {
                // ThirdPersonCamera setup: child named CameraPivot
                Transform found = transform.Find("CameraPivot");
                if (found != null) cameraPivot = found;
            }

            if (cameraPivot != null && cameraPivot.GetComponent<PlayerAimProvider>() == null)
                cameraPivot.gameObject.AddComponent<PlayerAimProvider>();

            PlayerMotor motor = GetComponent<PlayerMotor>();
            if (motor != null)
                motor.SetCameraPivot(cameraPivot);
        }
    }
}
