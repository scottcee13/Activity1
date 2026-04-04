using System;

[Serializable]
public class QuestStatus
{
    public string questID;
    public int currentAmount;
    public bool isCompleted;
    public bool rewardClaimed;

    public QuestStatus(string id)
    {
        questID = id;
        currentAmount = 0;
        isCompleted = false;
        rewardClaimed = false;
    }
}