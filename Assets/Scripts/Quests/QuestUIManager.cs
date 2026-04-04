using System.Collections.Generic;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public Transform questListParent;
    public GameObject questItemPrefab;

    private void Start()
    {
        RefreshUI();

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated += RefreshUI;
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestUpdated -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in questListParent)
        {
            Destroy(child.gameObject);
        }

        List<QuestInstance> quests = QuestManager.Instance.GetAllQuests();

        foreach (QuestInstance quest in quests)
        {
            GameObject item = Instantiate(questItemPrefab, questListParent);
            QuestUIItem uiItem = item.GetComponent<QuestUIItem>();
            uiItem.Setup(quest);
        }
    }
}