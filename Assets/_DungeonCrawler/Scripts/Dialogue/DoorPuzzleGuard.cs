using DungeonCrawler.Core;
using DungeonCrawler.Inventory;
using UnityEngine;

namespace DungeonCrawler.Dialogue
{
    /// <summary>
    /// Room 2 guard: requires key item + correct dialogue choice index to open door.
    /// </summary>
    public class DoorPuzzleGuard : MonoBehaviour
    {
        [SerializeField] private string requiredItemId = "ancient_key";
        [SerializeField] private string requiredHintId = "puzzle_hint";
        [SerializeField] private int correctChoiceIndex = 1;
        [SerializeField] private string dialogueIdToValidate = "guard_quiz";
        [SerializeField] private GameObject doorToOpen;
        [SerializeField] private DialogueDataSO guardDialogue;

        private int lastChoice = -1;

        private void OnEnable()
        {
            GameEvents.OnDialogueChoiceSelected += OnChoice;
        }

        private void OnDisable()
        {
            GameEvents.OnDialogueChoiceSelected -= OnChoice;
        }

        private void OnChoice(string dialogueId, int choiceIndex)
        {
            if (dialogueId != dialogueIdToValidate) return;
            lastChoice = choiceIndex;
            TryOpenDoor();
        }

        private void TryOpenDoor()
        {
            bool hasItem = InventoryManager.Instance != null &&
                           InventoryManager.Instance.HasItem(requiredItemId);

            bool hasHint = DialogueManager.Instance != null &&
                           DialogueManager.Instance.HasHint(requiredHintId);

            if (hasItem && hasHint && lastChoice == correctChoiceIndex)
            {
                if (doorToOpen != null) doorToOpen.SetActive(false);
                GameEvents.RaiseQuestObjectiveTriggered("puzzle_door_opened");
                Debug.Log("[DoorGuard] Door opened — correct answer!");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (Input.GetKeyDown(KeyCode.E) && guardDialogue != null)
                DialogueManager.Instance?.StartDialogue(guardDialogue);
        }
    }
}
