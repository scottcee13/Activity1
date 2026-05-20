using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Subscribes to GameEvents and forwards progress to legacy QuestManager.
    /// </summary>
    public class QuestEventBridge : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += OnEnemyKilled;
            GameEvents.OnDialogueEnded += OnDialogueEnded;
            GameEvents.OnItemCollected += OnItemCollected;
            GameEvents.OnQuestObjectiveTriggered += OnCustomObjective;
            GameEvents.OnAbilityUsed += OnAbilityUsedHandler;
            GameEvents.OnRoomEntered += OnRoomEntered;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= OnEnemyKilled;
            GameEvents.OnDialogueEnded -= OnDialogueEnded;
            GameEvents.OnItemCollected -= OnItemCollected;
            GameEvents.OnQuestObjectiveTriggered -= OnCustomObjective;
            GameEvents.OnAbilityUsed -= OnAbilityUsedHandler;
            GameEvents.OnRoomEntered -= OnRoomEntered;
        }

        private void OnEnemyKilled(string _) => AddProgress(ObjectiveType.Kill, 1);
        private void OnDialogueEnded(string _) => AddProgress(ObjectiveType.Dialogue, 1);
        private void OnItemCollected(string _) => AddProgress(ObjectiveType.Exploration, 1);
        private void OnRoomEntered(string _) => AddProgress(ObjectiveType.Exploration, 1);

        private void AddProgress(ObjectiveType type, int amount)
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.AddProgress(type, amount);
        }

        private void OnCustomObjective(string objectiveId)
        {
            Debug.Log($"[Quest] Custom objective: {objectiveId}");
            AddProgress(ObjectiveType.Exploration, 1);
        }

        private void OnAbilityUsedHandler(string abilityId)
        {
            if (abilityId == "dash" || abilityId == "jump")
                AddProgress(ObjectiveType.Exploration, 1);
        }
    }
}
