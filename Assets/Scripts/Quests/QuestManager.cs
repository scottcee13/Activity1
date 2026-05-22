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

    /// <summary>Raised after any quest state change (for UI/audio).</summary>
    public void NotifyQuestUpdated() => OnQuestUpdated?.Invoke();

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
            if (questData == null || string.IsNullOrEmpty(questData.questID)) continue;
            RegisterQuest(questData);
        }
    }

    public void RegisterQuest(QuestDataSO questData)
    {
        if (questData == null || string.IsNullOrEmpty(questData.questID)) return;
        if (questDict.ContainsKey(questData.questID)) return;

        QuestStatus status = new QuestStatus(questData.questID);
        questDict.Add(questData.questID, new QuestInstance(questData, status));
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

    /// <summary>Advance a single quest by id. Returns true if progress was applied.</summary>
    public bool AdvanceQuest(string questID, int amount = 1)
    {
        QuestInstance quest = GetQuest(questID);
        if (quest == null || quest.status.isCompleted || quest.status.rewardClaimed)
            return false;

        quest.objective.AddProgress(amount);
        OnQuestUpdated?.Invoke();
        return true;
    }

    public bool IsQuestComplete(string questID)
    {
        QuestInstance quest = GetQuest(questID);
        return quest != null && quest.status.isCompleted;
    }

    /// <summary>True if quest's configured target is empty or equals the event id.</summary>
    public static bool TargetMatches(QuestDataSO data, string eventId)
    {
        if (data == null) return false;

        string target = data.objectiveType switch
        {
            ObjectiveType.Kill => data.targetEntityId,
            ObjectiveType.Dialogue => data.targetDialogueId,
            ObjectiveType.Exploration => !string.IsNullOrEmpty(data.targetItemId)
                ? data.targetItemId
                : data.targetObjectiveId,
            _ => null
        };

        return string.IsNullOrEmpty(target) || target == eventId;
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

    public void ReportEnemyKilled(string enemyId)
    {
        if (!string.IsNullOrEmpty(enemyId))
            DungeonCrawler.Core.GameEvents.RaiseEnemyKilled(enemyId);
    }

    public void ReportBossKilled(string bossId = "dungeon_boss")
    {
        ReportEnemyKilled(bossId);
    }

    public void ReportItemCollected(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
            DungeonCrawler.Core.GameEvents.RaiseItemCollected(itemId);
    }

    public void ReportDialogueCompleted(string dialogueId)
    {
        if (!string.IsNullOrEmpty(dialogueId))
            DungeonCrawler.Core.GameEvents.RaiseDialogueEnded(dialogueId);
    }

    public void ReportObjectiveTriggered(string objectiveId)
    {
        if (!string.IsNullOrEmpty(objectiveId))
            DungeonCrawler.Core.GameEvents.RaiseQuestObjectiveTriggered(objectiveId);
    }

    public bool AdvanceObjective(string questId, int amount = 1) => AdvanceQuest(questId, amount);
}