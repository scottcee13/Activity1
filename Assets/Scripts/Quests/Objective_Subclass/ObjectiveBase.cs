public abstract class ObjectiveBase
{
    protected QuestDataSO questData;
    protected QuestStatus questStatus;

    public ObjectiveBase(QuestDataSO data, QuestStatus status)
    {
        questData = data;
        questStatus = status;
    }

    public virtual void AddProgress(int amount)
    {
        if (questStatus.isCompleted || questStatus.rewardClaimed) return;

        questStatus.currentAmount += amount;

        if (questStatus.currentAmount >= questData.requiredAmount)
        {
            questStatus.currentAmount = questData.requiredAmount;
            questStatus.isCompleted = true;
        }
    }

    public int GetCurrentAmount() => questStatus.currentAmount;
    public int GetRequiredAmount() => questData.requiredAmount;
    public bool IsCompleted() => questStatus.isCompleted;
}