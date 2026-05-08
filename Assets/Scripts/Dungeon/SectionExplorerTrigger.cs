using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SectionExplorerTrigger : MonoBehaviour
{
    [SerializeField] private int sectionIndex = 0;
    [SerializeField] private bool completeExplorationQuest = true;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        if (DungeonProgressManager.Instance != null &&
            DungeonProgressManager.Instance.CurrentSectionIndex == sectionIndex)
        {
            DungeonProgressManager.Instance.UnlockNextSection();
        }

        if (completeExplorationQuest && QuestManager.Instance != null)
        {
            QuestManager.Instance.AddProgress(ObjectiveType.Exploration, 1);
        }

        triggered = true;
    }
}
