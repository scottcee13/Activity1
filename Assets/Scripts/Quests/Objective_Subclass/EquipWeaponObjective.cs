public class EquipWeaponObjective : ObjectiveBase
{
    public EquipWeaponObjective(QuestDataSO data, QuestStatus status) : base(data, status)
    {
    }

    public override void AddProgress(int amount)
    {
        if (questStatus.isCompleted || questStatus.rewardClaimed) return;

        questStatus.currentAmount = questData.requiredAmount;
        questStatus.isCompleted = true;
    }
}
