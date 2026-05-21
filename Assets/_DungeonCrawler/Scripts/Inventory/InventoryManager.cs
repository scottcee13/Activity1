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
        [SerializeField] private ItemRegistry itemRegistry;

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

        public List<string> GetCollectedItemIds()
        {
            var ids = new List<string>();
            foreach (ItemDataSO item in items)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemId))
                    ids.Add(item.itemId);
            }
            return ids;
        }

        public void AddItem(ItemDataSO item, bool raiseEvents = true)
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

            if (raiseEvents)
            {
                OnInventoryChanged?.Invoke();
                GameEvents.RaiseItemCollected(item.itemId);
            }

            Debug.Log($"[Inventory] Added {item.displayName}");
        }

        public void LoadItemsFromIds(List<string> itemIds)
        {
            items.Clear();
            if (itemIds == null || itemRegistry == null) return;

            foreach (string id in itemIds)
            {
                ItemDataSO item = itemRegistry.GetById(id);
                if (item != null)
                    AddItem(item, raiseEvents: false);
            }

            OnInventoryChanged?.Invoke();
        }

        public void ClearItems()
        {
            items.Clear();
            OnInventoryChanged?.Invoke();
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
