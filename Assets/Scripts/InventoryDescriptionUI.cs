using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDescriptionUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public void DisplayItem(WeaponData weapon)
    {
        icon.sprite = weapon.weaponIcon;
        nameText.text = weapon.weaponName;
        descriptionText.text = weapon.weaponDescription;
    }
}