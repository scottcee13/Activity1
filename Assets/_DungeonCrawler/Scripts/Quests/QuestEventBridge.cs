using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Routes GameEvents to the active quest in QuestChainController with per-quest target filters.
    /// </summary>
    public class QuestEventBridge : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += OnEnemyKilled;
            GameEvents.OnDialogueEnded += OnDialogueEnded;
            GameEvents.OnDialogueStarted += OnDialogueStarted;
            GameEvents.OnItemCollected += OnItemCollected;
            GameEvents.OnQuestObjectiveTriggered += OnCustomObjective;
            GameEvents.OnRoomEntered += OnRoomEntered;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= OnEnemyKilled;
            GameEvents.OnDialogueEnded -= OnDialogueEnded;
            GameEvents.OnDialogueStarted -= OnDialogueStarted;
            GameEvents.OnItemCollected -= OnItemCollected;
            GameEvents.OnQuestObjectiveTriggered -= OnCustomObjective;
            GameEvents.OnRoomEntered -= OnRoomEntered;
        }

        private void OnEnemyKilled(string entityId)
        {
            TryProgressActive(ObjectiveType.Kill, entityId);

            if (entityId == "dungeon_boss")
                Core.GameManager.Instance?.TriggerVictory();
        }

        private void OnDialogueEnded(string dialogueId) =>
            TryProgressActive(ObjectiveType.Dialogue, dialogueId);

        private void OnDialogueStarted(string _) { }

        private void OnItemCollected(string itemId) =>
            TryProgressActive(ObjectiveType.Exploration, itemId, useItemTarget: true);

        private void OnCustomObjective(string objectiveId) =>
            TryProgressActive(ObjectiveType.Exploration, objectiveId, useItemTarget: false);

        private void OnRoomEntered(string roomId) =>
            TryProgressActive(ObjectiveType.Exploration, roomId, useItemTarget: false);

        private void TryProgressActive(ObjectiveType type, string eventId, bool useItemTarget = false)
        {
            if (QuestManager.Instance == null) return;

            string questId = QuestChainController.Instance != null
                ? QuestChainController.Instance.ActiveQuestId
                : null;

            if (!string.IsNullOrEmpty(questId))
            {
                QuestInstance quest = QuestManager.Instance.GetQuest(questId);
                if (quest != null && quest.data.objectiveType == type && MatchesEvent(quest.data, eventId, useItemTarget))
                {
                    if (QuestManager.Instance.AdvanceQuest(questId, 1))
                        RefreshQuestUi();
                    return;
                }
            }

            foreach (QuestInstance quest in QuestManager.Instance.GetAllQuests())
            {
                if (quest.status.isCompleted || quest.status.rewardClaimed) continue;
                if (quest.data.objectiveType != type) continue;
                if (!MatchesEvent(quest.data, eventId, useItemTarget)) continue;

                if (QuestManager.Instance.AdvanceQuest(quest.data.questID, 1))
                    RefreshQuestUi();
                return;
            }
        }

        private static void RefreshQuestUi()
        {
            QuestManager.Instance?.NotifyQuestUpdated();
            if (QuestUIManager.Instance != null)
                QuestUIManager.Instance.RefreshUI();
        }

        private static bool MatchesEvent(QuestDataSO data, string eventId, bool useItemTarget)
        {
            if (data.objectiveType == ObjectiveType.Exploration && useItemTarget)
                return QuestManager.TargetMatches(data, eventId);

            if (data.objectiveType == ObjectiveType.Exploration && !useItemTarget)
            {
                if (!string.IsNullOrEmpty(data.targetItemId)) return false;
                return QuestManager.TargetMatches(data, eventId);
            }

            return QuestManager.TargetMatches(data, eventId);
        }
    }
}
