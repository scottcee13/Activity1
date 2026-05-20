using System;
using System.Collections.Generic;
using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Inventory
{
    /// <summary>
    /// Unified inventory: quest items + weapons. Syncs with legacy PlayerInventory for saves.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField] private ItemDatabase legacyDatabase;

        private readonly List<ItemDataSO> items = new List<ItemDataSO>();
        private WeaponData equippedWeapon;

        public event Action OnInventoryChanged;

        public WeaponData EquippedWeapon => equippedWeapon;
        public IReadOnlyList<ItemDataSO> Items => items;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public bool HasItem(string itemId)
        {
            foreach (ItemDataSO item in items)
            {
                if (item != null && item.itemId == itemId) return true;
            }
            return false;
        }

        public void AddItem(ItemDataSO item)
        {
            if (item == null) return;
            if (items.Contains(item)) return;

            items.Add(item);

            if (item.linkedWeapon != null)
            {
                if (PlayerInventory.instance != null)
                    PlayerInventory.instance.AddWeapon(item.linkedWeapon);

                EquipWeapon(item.linkedWeapon);
            }

            OnInventoryChanged?.Invoke();
            GameEvents.RaiseItemCollected(item.itemId);
            Debug.Log($"[Inventory] Added {item.displayName}");
        }

        public void EquipWeapon(WeaponData weapon)
        {
            if (weapon == null) return;
            equippedWeapon = weapon;
            GameEvents.RaiseWeaponEquipped(weapon);
            OnInventoryChanged?.Invoke();
        }

        public void EquipWeaponById(string weaponId)
        {
            if (legacyDatabase == null) return;
            WeaponData w = legacyDatabase.GetWeaponByID(weaponId);
            if (w != null) EquipWeapon(w);
        }
    }
}
