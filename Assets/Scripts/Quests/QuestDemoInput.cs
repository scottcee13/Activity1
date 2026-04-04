using UnityEngine;

public class QuestDemoInput : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            QuestManager.Instance.AddProgress(ObjectiveType.Kill, 1);
            Debug.Log("Kill progress +1");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            QuestManager.Instance.AddProgress(ObjectiveType.Dialogue, 1);
            Debug.Log("Dialogue progress +1");
        }
    }
}