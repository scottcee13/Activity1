using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapons/Weapon Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField]
    private List<WeaponData> weapons = new List<WeaponData>();

    public List<WeaponData> Weapons
    {
        get { return weapons; }
    }
}