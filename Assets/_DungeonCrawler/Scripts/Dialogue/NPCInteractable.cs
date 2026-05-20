using UnityEngine;

namespace DungeonCrawler.Dialogue
{
    /// <summary>
    /// Press E near NPC to start dialogue. Shows optional prompt object.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NPCInteractable : MonoBehaviour
    {
        [SerializeField] private DialogueDataSO dialogue;
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private bool oneShot;
        private bool used;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (oneShot && used) return;

            if (interactPrompt != null) interactPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
                Interact();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }

        public void Interact()
        {
            if (dialogue == null || DialogueManager.Instance == null) return;
            if (oneShot && used) return;

            DialogueManager.Instance.StartDialogue(dialogue);
            used = true;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayVoice(null);
        }
    }
}
