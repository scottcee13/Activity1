using DungeonCrawler.Core;
using DungeonCrawler.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeapon : MonoBehaviour
{
    [FormerlySerializedAs("weaponObjects")]
    [SerializeField] private WeaponData[] weaponSlots = new WeaponData[3];
    [SerializeField] private WeaponEquipManager equipManager;

    private int selectedSlot;

    public bool IsEquipped => equipManager != null && equipManager.IsWeaponEquipped;

    private void Awake()
    {
        if (equipManager == null)
            equipManager = GetComponent<WeaponEquipManager>();

        if (weaponSlots == null || weaponSlots.Length < 3)
            weaponSlots = new WeaponData[3];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleEquip();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(2);
    }

    public void ToggleEquip()
    {
        if (equipManager == null) return;

        if (equipManager.IsWeaponEquipped)
        {
            equipManager.Unequip();
            return;
        }

        WeaponData weapon = GetSlotWeapon(selectedSlot);
        if (weapon != null)
            equipManager.Equip(weapon);
    }

    public void SelectSlot(int index)
    {
        if (weaponSlots == null || index < 0 || index >= weaponSlots.Length) return;

        selectedSlot = index;
        WeaponData weapon = weaponSlots[index];
        if (weapon == null || equipManager == null) return;

        if (equipManager.IsWeaponEquipped && equipManager.CurrentWeapon == weapon)
            return;

        equipManager.Equip(weapon);
    }

    public void UnequipWeapon()
    {
        equipManager?.Unequip();
    }

    public WeaponData GetCurrentWeapon()
    {
        if (equipManager != null && equipManager.CurrentWeapon != null)
            return equipManager.CurrentWeapon;

        return GetSlotWeapon(selectedSlot);
    }

    public WeaponData GetSlotWeapon(int index)
    {
        if (weaponSlots == null || index < 0 || index >= weaponSlots.Length) return null;
        return weaponSlots[index];
    }
}
