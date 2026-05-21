using System;
using System.Collections.Generic;
using DungeonCrawler.Core;
using DungeonCrawler.Quests;
using DungeonCrawler.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonCrawler.Dialogue
{
    [Serializable]
    public class QuestDialogueEntry
    {
        [Tooltip("Quest id this line belongs to (e.g. tutorial_talk)")]
        public string questId;
        [Tooltip("If true, show after that quest is completed; if false, show while it is active")]
        public bool whenQuestComplete;
        public DialogueDataSO dialogue;
    }

    /// <summary>
    /// Press E near NPC to start dialogue. Picks dialogue by quest chain state.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NPCInteractable : MonoBehaviour
    {
        [FormerlySerializedAs("dialogue")]
        [SerializeField] private DialogueDataSO defaultDialogue;
        [SerializeField] private List<QuestDialogueEntry> questDialogues = new List<QuestDialogueEntry>();
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private string promptMessage = "Press E to talk";
        [SerializeField] private bool oneShotDefault;

        private bool defaultUsed;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsActive) return;

            if (interactPrompt != null)
                interactPrompt.SetActive(true);
            else if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Show(promptMessage);

            if (Input.GetKeyDown(KeyCode.E))
                Interact();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (interactPrompt != null)
                interactPrompt.SetActive(false);
            else if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Hide();
        }

        public void Interact()
        {
            if (DialogueManager.Instance == null) return;
            if (DialogueManager.Instance.IsActive) return;

            DialogueDataSO dialogue = ResolveDialogue();
            if (dialogue == null) return;

            if (oneShotDefault && dialogue == defaultDialogue && defaultUsed) return;

            DialogueManager.Instance.StartDialogue(dialogue);

            if (dialogue == defaultDialogue)
                defaultUsed = true;

            if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.ForceHide();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayVoice(null);
        }

        private DialogueDataSO ResolveDialogue()
        {
            if (questDialogues != null && questDialogues.Count > 0 && QuestManager.Instance != null)
            {
                string activeId = QuestChainController.Instance != null
                    ? QuestChainController.Instance.ActiveQuestId
                    : null;

                foreach (QuestDialogueEntry entry in questDialogues)
                {
                    if (entry?.dialogue == null || string.IsNullOrEmpty(entry.questId)) continue;

                    if (entry.whenQuestComplete)
                    {
                        if (QuestManager.Instance.IsQuestComplete(entry.questId))
                            return entry.dialogue;
                    }
                    else if (entry.questId == activeId)
                    {
                        return entry.dialogue;
                    }
                }
            }

            return defaultDialogue;
        }
    }
}
