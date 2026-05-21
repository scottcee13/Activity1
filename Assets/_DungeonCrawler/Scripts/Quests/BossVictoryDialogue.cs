using DungeonCrawler.Core;
using DungeonCrawler.Dialogue;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    public class BossVictoryDialogue : MonoBehaviour
    {
        [SerializeField] private string bossEntityId = "dungeon_boss";
        [SerializeField] private DialogueDataSO endingDialogue;
        [SerializeField] private bool triggerVictoryAfterDialogue = true;
        [SerializeField] private bool victoryIfNoDialogue = true;

        private bool played;
        private string pendingDialogueId;

        private void OnEnable() => GameEvents.OnEnemyKilled += OnEnemyKilled;
        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= OnEnemyKilled;
            GameEvents.OnDialogueEnded -= OnEndingDialogueFinished;
        }

        private void OnEnemyKilled(string entityId)
        {
            if (played || entityId != bossEntityId) return;
            played = true;

            if (endingDialogue == null || DialogueManager.Instance == null)
            {
                if (victoryIfNoDialogue)
                    GameManager.Instance?.TriggerVictory();
                return;
            }

            pendingDialogueId = endingDialogue.dialogueId;
            DialogueManager.Instance.StartDialogue(endingDialogue);

            if (triggerVictoryAfterDialogue)
                GameEvents.OnDialogueEnded += OnEndingDialogueFinished;
            else
                GameManager.Instance?.TriggerVictory();
        }

        private void OnEndingDialogueFinished(string dialogueId)
        {
            if (dialogueId != pendingDialogueId) return;

            GameEvents.OnDialogueEnded -= OnEndingDialogueFinished;
            GameManager.Instance?.TriggerVictory();
        }
    }
}
