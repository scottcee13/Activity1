using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Inventory
{
    [CreateAssetMenu(fileName = "ItemRegistry", menuName = "Dungeon/Item Registry")]
    public class ItemRegistry : ScriptableObject
    {
        [SerializeField] private List<ItemDataSO> items = new List<ItemDataSO>();

        public ItemDataSO GetById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;

            foreach (ItemDataSO item in items)
            {
                if (item != null && item.itemId == itemId)
                    return item;
            }

            return null;
        }
    }
}
