using System.IO;
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
        LoadGame(); // AUTO LOAD
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();

        foreach (var weapon in PlayerInventory.instance.ownedWeapons)
        {
            data.ownedWeaponIDs.Add(weapon.weaponID);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log(path);
    }

    public void LoadGame()
    {
        if (!File.Exists(path)) return;

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        PlayerInventory.instance.ownedWeapons.Clear();

        foreach (string id in data.ownedWeaponIDs)
        {
            WeaponData weapon = database.GetWeaponByID(id);
            PlayerInventory.instance.AddWeapon(weapon);
        }

        Debug.Log("Loaded weapons: " + data.ownedWeaponIDs.Count);
    }

    public void ResetSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        PlayerInventory.instance.ownedWeapons.Clear();
        Debug.Log("Inventory Reset");
    }
}