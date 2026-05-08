using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueHint : MonoBehaviour
{
    [TextArea] public string[] dialogueLines;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Transform player;

    private int lineIndex;

    private void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;
        if (Vector3.Distance(player.position, transform.position) > interactDistance) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            ShowNextLine();
            QuestManager.Instance?.AddProgress(ObjectiveType.Dialogue, 1);
        }
    }

    private void ShowNextLine()
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        if (dialogueText != null)
        {
            dialogueText.text = dialogueLines[lineIndex];
            lineIndex = (lineIndex + 1) % dialogueLines.Length;
        }
    }
}
