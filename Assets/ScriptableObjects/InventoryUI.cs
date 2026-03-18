using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public ItemDatabase database;
    public GameObject slotPrefab;
    public Transform gridParent;

    public InventoryDescriptionUI descriptionUI;

    private void OnEnable()
    {
        DisplayWeapons();
    }

    void DisplayWeapons()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (WeaponData weapon in PlayerInventory.instance.ownedWeapons)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();
            slotScript.Setup(weapon, descriptionUI);
        }
    }
}