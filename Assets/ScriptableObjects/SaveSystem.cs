using System.IO;
using DungeonCrawler.Inventory;
using DungeonCrawler.Quests;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public ItemDatabase database;
    public static SaveSystem instance;
    string path;

    void Awake()
    {
        path = Application.persistentDataPath + "/save.json";

        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        if (PlayerInventory.instance == null)
        {
            Debug.LogWarning("Save failed: PlayerInventory.instance is null");
            return;
        }

        SaveData data = new SaveData();

        foreach (var weapon in PlayerInventory.instance.ownedWeapons)
            data.ownedWeaponIDs.Add(weapon.weaponID);

        if (QuestManager.Instance != null)
            data.questStatuses = QuestManager.Instance.GetQuestStatuses();

        if (InventoryManager.Instance != null)
            data.collectedItemIds = InventoryManager.Instance.GetCollectedItemIds();

        if (QuestChainController.Instance != null)
            data.questChainIndex = QuestChainController.Instance.GetChainIndexForSave();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log("Game saved to: " + path);
    }

    public void LoadGame()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No save file found at: " + path);
            return;
        }

        if (database == null)
        {
            Debug.LogWarning("Load failed: database is null");
            return;
        }

        if (PlayerInventory.instance == null)
        {
            Debug.LogWarning("Load failed: PlayerInventory.instance is null");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        PlayerInventory.instance.ownedWeapons.Clear();

        foreach (string id in data.ownedWeaponIDs)
        {
            WeaponData weapon = database.GetWeaponByID(id);
            if (weapon != null)
                PlayerInventory.instance.AddWeapon(weapon);
            else
                Debug.LogWarning("Weapon not found in database: " + id);
        }

        if (QuestManager.Instance != null && data.questStatuses != null)
            QuestManager.Instance.LoadQuestStatuses(data.questStatuses);

        if (InventoryManager.Instance != null && data.collectedItemIds != null)
            InventoryManager.Instance.LoadItemsFromIds(data.collectedItemIds);

        if (QuestChainController.Instance != null)
            QuestChainController.Instance.SyncChainIndexFromQuests();

        Debug.Log($"Loaded save — weapons: {data.ownedWeaponIDs.Count}, quests: {data.questStatuses?.Count ?? 0}, items: {data.collectedItemIds?.Count ?? 0}");
    }

    public void ResetSave()
    {
        if (File.Exists(path))
            File.Delete(path);

        if (PlayerInventory.instance != null)
            PlayerInventory.instance.ownedWeapons.Clear();

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ClearItems();

        if (QuestManager.Instance != null)
            QuestManager.Instance.ResetAllQuestProgress();

        if (QuestChainController.Instance != null)
            QuestChainController.Instance.LoadChainIndex(0);

        Debug.Log("Save file deleted and runtime data reset.");
    }
}
