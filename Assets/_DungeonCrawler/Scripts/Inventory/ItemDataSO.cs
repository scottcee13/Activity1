using UnityEngine;

namespace DungeonCrawler.Inventory
{
    public enum ItemType
    {
        Quest,
        Weapon,
        Consumable,
        Key
    }

    [CreateAssetMenu(fileName = "ItemData", menuName = "Dungeon/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        public string itemId;
        public string displayName;
        [TextArea] public string description;
        public ItemType itemType = ItemType.Quest;
        public Sprite icon;

        [Header("Weapon link (optional)")]
        public WeaponData linkedWeapon;
    }
}
