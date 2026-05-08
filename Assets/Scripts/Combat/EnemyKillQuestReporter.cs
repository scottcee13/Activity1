using UnityEngine;

public class EnemyKillQuestReporter : MonoBehaviour
{
    private void OnEnable()
    {
        EnemyHealth.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void HandleEnemyKilled(EnemyHealth enemy)
    {
        if (QuestManager.Instance == null) return;
        QuestManager.Instance.AddProgress(ObjectiveType.Kill, 1);
    }
}
