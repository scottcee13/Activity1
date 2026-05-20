using System.Collections.Generic;
using DungeonCrawler.Core;
using DungeonCrawler.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.Dialogue
{
    /// <summary>
    /// Plays branching dialogue, pauses gameplay, notifies quests via GameEvents.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private Transform choicesParent;
        [SerializeField] private Button choiceButtonPrefab;

        private DialogueDataSO currentDialogue;
        private int currentNodeIndex;
        private int currentLineIndex;
        private readonly HashSet<string> collectedHints = new HashSet<string>();

        public bool IsActive { get; private set; }

        private void Awake()
        {
            Instance = this;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        public bool HasHint(string hintId) => collectedHints.Contains(hintId);

        public void StartDialogue(DialogueDataSO dialogue)
        {
            if (dialogue == null || dialogue.nodes.Count == 0) return;

            currentDialogue = dialogue;
            currentNodeIndex = 0;
            currentLineIndex = 0;
            IsActive = true;

            if (dialoguePanel != null) dialoguePanel.SetActive(true);
            if (GameManager.Instance != null) GameManager.Instance.SetGameplayPaused(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            ShowCurrentLine();
        }

        public void AdvanceLine()
        {
            if (!IsActive || currentDialogue == null) return;

            DialogueNode node = currentDialogue.nodes[currentNodeIndex];
            currentLineIndex++;

            if (currentLineIndex >= node.lines.Count)
            {
                if (node.choices.Count > 0)
                    ShowChoices(node);
                else if (node.endAfterLines)
                    EndDialogue();
                else
                    EndDialogue();
                return;
            }

            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            ClearChoices();
            DialogueNode node = currentDialogue.nodes[currentNodeIndex];
            DialogueLine line = node.lines[currentLineIndex];

            if (speakerText != null) speakerText.text = line.speakerName;
            if (bodyText != null) bodyText.text = line.text;
        }

        private void ShowChoices(DialogueNode node)
        {
            ClearChoices();
            if (bodyText != null) bodyText.text = "Choose your response:";

            for (int i = 0; i < node.choices.Count; i++)
            {
                int index = i;
                DialogueChoice choice = node.choices[i];
                Button btn = Instantiate(choiceButtonPrefab, choicesParent);
                TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = choice.choiceText;
                btn.onClick.AddListener(() => SelectChoice(index, choice));
            }
        }

        private void SelectChoice(int choiceIndex, DialogueChoice choice)
        {
            GameEvents.RaiseDialogueChoice(currentDialogue.dialogueId, choiceIndex);

            if (!string.IsNullOrEmpty(choice.grantItemId))
            {
                // Designer assigns items via pickup SO lookup in scene, or extend ItemDatabase
                GameEvents.RaiseQuestObjectiveTriggered(choice.questTriggerId);
            }

            if (!string.IsNullOrEmpty(choice.questTriggerId))
                GameEvents.RaiseQuestObjectiveTriggered(choice.questTriggerId);

            if (choice.nextNodeIndex >= 0 && choice.nextNodeIndex < currentDialogue.nodes.Count)
            {
                currentNodeIndex = choice.nextNodeIndex;
                currentLineIndex = 0;
                ShowCurrentLine();
            }
            else
            {
                EndDialogue();
            }
        }

        public void EndDialogue()
        {
            if (currentDialogue != null)
            {
                if (!string.IsNullOrEmpty(currentDialogue.grantsHintId))
                    collectedHints.Add(currentDialogue.grantsHintId);

                GameEvents.RaiseDialogueEnded(currentDialogue.dialogueId);
            }

            IsActive = false;
            currentDialogue = null;
            ClearChoices();

            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.SetGameplayPaused(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void ClearChoices()
        {
            if (choicesParent == null) return;
            foreach (Transform child in choicesParent)
                Destroy(child.gameObject);
        }

        private void Update()
        {
            if (!IsActive) return;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (choicesParent != null && choicesParent.childCount == 0)
                    AdvanceLine();
            }
        }
    }
}
