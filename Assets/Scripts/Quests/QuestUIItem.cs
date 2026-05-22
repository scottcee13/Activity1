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

    public void Setup(QuestInstance quest, bool isActiveQuest = false)
    {
        if (quest == null || quest.data == null) return;

        questID = quest.data.questID;

        if (titleText != null)
            titleText.text = isActiveQuest ? $"► {quest.data.questTitle}" : quest.data.questTitle;

        if (descriptionText != null)
            descriptionText.text = quest.data.description;

        if (progressText != null)
        {
            if (quest.status.isCompleted)
            {
                progressText.text = quest.data.objectiveType == ObjectiveType.EquipWeapon
                    ? "Quest Complete"
                    : quest.status.rewardClaimed ? "Completed" : "Complete — claim reward";
            }
            else if (quest.data.objectiveType == ObjectiveType.EquipWeapon)
            {
                progressText.text = quest.data.description;
            }
            else
            {
                progressText.text = $"{quest.status.currentAmount} / {quest.data.requiredAmount}";
            }
        }

        if (claimButton != null)
        {
            bool showClaim = quest.status.isCompleted && !quest.status.rewardClaimed;
            claimButton.gameObject.SetActive(showClaim);
            claimButton.onClick.RemoveAllListeners();
            if (showClaim)
                claimButton.onClick.AddListener(ClaimReward);
        }

        if (canvasGroup != null)
        {
            if (quest.status.rewardClaimed)
                canvasGroup.alpha = 0.45f;
            else if (quest.status.isCompleted)
                canvasGroup.alpha = 0.75f;
            else if (isActiveQuest)
                canvasGroup.alpha = 1f;
            else
                canvasGroup.alpha = 0.85f;
        }
    }

    private void ClaimReward()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.ClaimReward(questID);
    }
}
