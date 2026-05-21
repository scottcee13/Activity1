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

    [Header("Targets (optional — empty = any)")]
    [Tooltip("Kill quests: HealthComponent entity id (e.g. first_enemy, dungeon_boss)")]
    public string targetEntityId;
    [Tooltip("Dialogue quests: DialogueDataSO dialogueId")]
    public string targetDialogueId;
    [Tooltip("Item quests: ItemDataSO itemId")]
    public string targetItemId;
    [Tooltip("Zone quests: QuestObjectiveTrigger objectiveId")]
    public string targetObjectiveId;

    [Header("Rewards")]
    public int goldReward = 0;
    public int expReward = 0;
}