using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> ownedWeaponIDs = new List<string>();
    public List<QuestStatus> questStatuses = new List<QuestStatus>();
    public List<string> collectedItemIds = new List<string>();
    public int questChainIndex;
}