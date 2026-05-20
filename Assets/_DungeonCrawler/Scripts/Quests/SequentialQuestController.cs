using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Tutorial flow: completes quests in order by questID list.
    /// Attach to Room 1 and call ActivateNext() from triggers/dialogue.
    /// </summary>
    public class SequentialQuestController : MonoBehaviour
    {
        [SerializeField] private List<string> questIdsInOrder = new List<string>();
        private int currentIndex;

        public void ActivateNext()
        {
            if (currentIndex >= questIdsInOrder.Count) return;

            string questId = questIdsInOrder[currentIndex];
            QuestInstance quest = QuestManager.Instance?.GetQuest(questId);
            if (quest != null && !quest.status.isCompleted)
            {
                quest.objective.AddProgress(quest.data.requiredAmount);
                QuestManager.Instance.OnQuestUpdated?.Invoke();
            }

            currentIndex++;
        }

        public void OnTutorialStep(int stepIndex)
        {
            while (currentIndex < stepIndex && currentIndex < questIdsInOrder.Count)
                ActivateNext();
        }
    }
}
