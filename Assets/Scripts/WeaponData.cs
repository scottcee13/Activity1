using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponID;
    public string weaponName;
    public int weaponDamage = 10;
    public string weaponDescription;
    public float attackCooldown = 0.5f;
    public float knockbackForce = 12f;
    public Sprite weaponIcon;

    [Header("3D equip")]
    public GameObject weaponPrefab;
    public Vector3 gripLocalPosition;
    public Vector3 gripLocalEuler;

    [Header("Combat audio")]
    public AudioClip swingSfx;
    public AudioClip hitSfx;
}
