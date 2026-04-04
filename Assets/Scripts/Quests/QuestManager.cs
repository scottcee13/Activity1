using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Database")]
    public List<QuestDataSO> allQuestData = new List<QuestDataSO>();

    private Dictionary<string, QuestInstance> questDict = new Dictionary<string, QuestInstance>();

    public Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeQuests();
    }

    private void InitializeQuests()
    {
        questDict.Clear();

        foreach (QuestDataSO questData in allQuestData)
        {
            QuestStatus status = new QuestStatus(questData.questID);
            QuestInstance instance = new QuestInstance(questData, status);
            questDict.Add(questData.questID, instance);
        }
    }

    public List<QuestInstance> GetAllQuests()
    {
        return new List<QuestInstance>(questDict.Values);
    }

    public QuestInstance GetQuest(string questID)
    {
        if (questDict.ContainsKey(questID))
            return questDict[questID];

        return null;
    }

    public void AddProgress(ObjectiveType type, int amount)
    {
        foreach (var pair in questDict)
        {
            QuestInstance quest = pair.Value;

            if (quest.data.objectiveType == type && !quest.status.rewardClaimed)
            {
                quest.objective.AddProgress(amount);
            }
        }

        OnQuestUpdated?.Invoke();
    }

    public void ClaimReward(string questID)
    {
        QuestInstance quest = GetQuest(questID);
        if (quest == null) return;
        if (!quest.status.isCompleted) return;
        if (quest.status.rewardClaimed) return;

        quest.status.rewardClaimed = true;

        Debug.Log($"Reward claimed for {quest.data.questTitle}! Gold: {quest.data.goldReward}, EXP: {quest.data.expReward}");

        OnQuestUpdated?.Invoke();
    }

    public List<QuestStatus> GetQuestStatuses()
    {
        List<QuestStatus> statuses = new List<QuestStatus>();

        foreach (var pair in questDict)
        {
            statuses.Add(pair.Value.status);
        }

        return statuses;
    }

    public void LoadQuestStatuses(List<QuestStatus> loadedStatuses)
    {
        foreach (QuestStatus loadedStatus in loadedStatuses)
        {
            if (questDict.ContainsKey(loadedStatus.questID))
            {
                questDict[loadedStatus.questID].status.currentAmount = loadedStatus.currentAmount;
                questDict[loadedStatus.questID].status.isCompleted = loadedStatus.isCompleted;
                questDict[loadedStatus.questID].status.rewardClaimed = loadedStatus.rewardClaimed;
            }
        }

        OnQuestUpdated?.Invoke();
    }

    public void ResetAllQuestProgress()
    {
        foreach (var pair in questDict)
        {
            QuestInstance quest = pair.Value;
            quest.status.currentAmount = 0;
            quest.status.isCompleted = false;
            quest.status.rewardClaimed = false;
        }

        OnQuestUpdated?.Invoke();
    }
}