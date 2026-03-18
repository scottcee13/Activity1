using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    public List<WeaponData> ownedWeapons = new List<WeaponData>();

    private void Awake()
    {
        instance = this;
    }

    public void AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("Tried to add NULL weapon!");
            return;
        }

        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            Debug.Log("Bought " + weapon.weaponName);
        }
    }
}