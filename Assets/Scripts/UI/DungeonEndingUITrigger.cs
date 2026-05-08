using UnityEngine;

public class DungeonEndingUITrigger : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private bool unlockCursor = true;
    private bool finished;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;
        if (endingPanel != null) endingPanel.SetActive(true);

        if (unlockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
