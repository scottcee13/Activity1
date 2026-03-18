using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public ItemDatabase database;
    public GameObject slotPrefab;
    public Transform gridParent;
    public ShopDescriptionUI descriptionUI;

    void Start()
    {
        DisplayShop();
    }

    void DisplayShop()
    {
        foreach (WeaponData weapon in database.Weapons)
        {
            GameObject slot = Instantiate(slotPrefab, gridParent);

            ShopSlot shopSlot = slot.GetComponent<ShopSlot>();
            shopSlot.Setup(weapon, descriptionUI);
        }
    }
}