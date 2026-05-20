using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    public class QuestObjectiveTrigger : MonoBehaviour
    {
        [SerializeField] private string objectiveId;
        [SerializeField] private bool oneShot = true;
        private bool fired;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (oneShot && fired) return;

            fired = true;
            GameEvents.RaiseQuestObjectiveTriggered(objectiveId);
        }
    }
}
