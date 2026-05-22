using DungeonCrawler.UI;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.World
{
    /// <summary>
    /// Press E to open a gate. Supports rotation or animator-driven doors.
    /// </summary>
    public class InteractableGate : MonoBehaviour
    {
        public enum OpenMode { Rotate, Animator }

        [SerializeField] private OpenMode openMode = OpenMode.Rotate;
        [SerializeField] private Transform doorLeaf;
        [SerializeField] private Animator doorAnimator;
        [SerializeField] private string openTrigger = "Open";
        [SerializeField] private Vector3 closedEuler;
        [SerializeField] private Vector3 openEuler;
        [SerializeField] private float openDuration = 1.2f;
        [SerializeField] private string promptMessage = "Press E to open gate";
        [SerializeField] private Collider blockingCollider;
        [SerializeField] private NavMeshObstacle navObstacle;
        [SerializeField] private bool oneShot = true;

        private bool isOpen;
        private bool isAnimating;
        private float openT;
        private Quaternion closedRot;
        private Quaternion openRot;

        private void Awake()
        {
            if (doorLeaf == null)
                doorLeaf = transform;

            closedRot = Quaternion.Euler(closedEuler);
            openRot = Quaternion.Euler(openEuler);
            doorLeaf.localRotation = closedRot;

            if (blockingCollider == null)
                blockingCollider = GetComponent<Collider>();

            if (navObstacle == null)
                navObstacle = GetComponent<NavMeshObstacle>();
        }

        private void Update()
        {
            if (openMode != OpenMode.Rotate || !isAnimating) return;

            openT += Time.deltaTime / Mathf.Max(0.01f, openDuration);
            doorLeaf.localRotation = Quaternion.Slerp(closedRot, openRot, openT);

            if (openT >= 1f)
            {
                isAnimating = false;
                ReleaseBlocking();
            }
        }

        public void HandlePlayerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (isOpen && oneShot) return;
            if (isAnimating) return;

            if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Show(promptMessage);

            if (Input.GetKeyDown(KeyCode.E))
                OpenGate();
        }

        public void HandlePlayerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Hide();
        }

        private void OnTriggerStay(Collider other) => HandlePlayerStay(other);
        private void OnTriggerExit(Collider other) => HandlePlayerExit(other);

        public void OpenGate()
        {
            if (isOpen && oneShot) return;
            if (isAnimating) return;

            isOpen = true;
            isAnimating = openMode == OpenMode.Rotate;
            openT = 0f;

            if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.ForceHide();

            if (openMode == OpenMode.Animator && doorAnimator != null)
            {
                doorAnimator.SetTrigger(openTrigger);
                ReleaseBlocking();
                isAnimating = false;
            }
            else if (openMode == OpenMode.Rotate)
            {
                if (openDuration <= 0f)
                {
                    doorLeaf.localRotation = openRot;
                    ReleaseBlocking();
                    isAnimating = false;
                }
            }
        }

        private void ReleaseBlocking()
        {
            if (blockingCollider != null)
                blockingCollider.enabled = false;

            if (navObstacle != null)
                navObstacle.enabled = false;
        }
    }
}
