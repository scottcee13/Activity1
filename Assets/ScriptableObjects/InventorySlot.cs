using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    private WeaponData weapon;

    public void Setup(WeaponData newWeapon)
    {
        weapon = newWeapon;
        icon.sprite = weapon.weaponIcon;
    }

    public void OnClick()
    {
        Debug.Log(weapon.weaponName + " - " + weapon.weaponDescription);
    }
}