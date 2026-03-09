using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    private WeaponData weapon;
    public GameObject highlight;
    private InventoryDescriptionUI descriptionUI;

    public void Setup(WeaponData newWeapon, InventoryDescriptionUI descUI)
    {
        weapon = newWeapon;
        icon.sprite = weapon.weaponIcon;
        descriptionUI = descUI;

        highlight.SetActive(false);
    }

    public void OnClick()

    {
        descriptionUI.DisplayItem(weapon);

        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();

        foreach (InventorySlot slot in allSlots)
        {
            slot.highlight.SetActive(false);
        }

        highlight.SetActive(true);


        Debug.Log(weapon.weaponName + " - " + weapon.weaponDescription);
    }
}