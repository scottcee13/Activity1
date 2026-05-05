using UnityEngine;

public enum ObjectiveType
{
    Kill,
    Dialogue,
    Exploration
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest Data")]
public class QuestDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string questID;
    public string questTitle;
    [TextArea] public string description;

    [Header("Objective")]
    public ObjectiveType objectiveType;
    public int requiredAmount = 1;

    [Header("Rewards")]
    public int goldReward = 0;
    public int expReward = 0;
}