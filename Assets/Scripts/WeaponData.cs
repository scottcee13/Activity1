using UnityEngine;


[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponID;
    public string weaponName;
    public int weaponDamage;
    public string weaponDescription;
    public float attackCooldown;
    public Sprite weaponIcon;
}
