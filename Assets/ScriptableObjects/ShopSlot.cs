using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon;

    private WeaponData weapon;
    private ShopDescriptionUI shopDescription;

    public void Setup(WeaponData newWeapon, ShopDescriptionUI descriptionUI)
    {
        weapon = newWeapon;
        shopDescription = descriptionUI;
        icon.sprite = weapon.weaponIcon;
    }

    public void SelectWeapon()
    {
        shopDescription.DisplayShopItem(weapon);
    }
}