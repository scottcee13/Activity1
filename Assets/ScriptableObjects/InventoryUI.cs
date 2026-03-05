using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public ItemDatabase database;
    public GameObject slotPrefab;
    public Transform gridParent;

    private void Start()
    {
        DisplayWeapons();
    }

    void DisplayWeapons()
    {
        foreach (WeaponData weapon in database.Weapons)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);

            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            slotScript.Setup(weapon);
        }
    }
}