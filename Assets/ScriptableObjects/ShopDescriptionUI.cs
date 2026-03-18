using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopDescriptionUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public Button buyButton; // Reference to the actual button in the panel

    private WeaponData currentWeapon;

    public void DisplayShopItem(WeaponData weapon)
    {
        currentWeapon = weapon;

        icon.sprite = weapon.weaponIcon;
        nameText.text = weapon.weaponName;
        descriptionText.text = weapon.weaponDescription;

        // Show the panel if it was hidden
        gameObject.SetActive(true);
    }

    // This is called by the "Buy" button's OnClick event
    public void BuyWeapon()
    {
        if (currentWeapon != null)
        {
            PlayerInventory.instance.AddWeapon(currentWeapon);
            SaveSystem.instance.SaveGame();
            Debug.Log("Purchased: " + currentWeapon.weaponName);
        }
    }
}