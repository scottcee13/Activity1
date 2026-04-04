using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIItem : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Button claimButton;
    public CanvasGroup canvasGroup;

    private string questID;

    public void Setup(QuestInstance quest)
    {
        questID = quest.data.questID;

        titleText.text = quest.data.questTitle;
        descriptionText.text = quest.data.description;
        progressText.text = $"{quest.status.currentAmount}/{quest.data.requiredAmount}";

        claimButton.gameObject.SetActive(quest.status.isCompleted && !quest.status.rewardClaimed);
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(ClaimReward);

        if (quest.status.rewardClaimed)
        {
            canvasGroup.alpha = 0.5f;
            claimButton.gameObject.SetActive(false);
            progressText.text = "Completed";
        }
        else if (quest.status.isCompleted)
        {
            canvasGroup.alpha = 0.7f;
            progressText.text = "Completed - Ready to Claim";
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void ClaimReward()
    {
        QuestManager.Instance.ClaimReward(questID);
    }
}