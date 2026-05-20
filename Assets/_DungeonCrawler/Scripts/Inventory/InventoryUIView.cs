using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.Inventory
{
    /// <summary>
    /// Displays inventory items and equipped weapon. Hook to UIManager toggle.
    /// </summary>
    public class InventoryUIView : MonoBehaviour
    {
        [SerializeField] private Transform listParent;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private Image equippedWeaponIcon;
        [SerializeField] private TMP_Text equippedWeaponName;

        private void OnEnable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged += Refresh;

            Refresh();
        }

        private void OnDisable()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged -= Refresh;
        }

        public void Refresh()
        {
            if (listParent == null || InventoryManager.Instance == null) return;

            foreach (Transform child in listParent)
                Destroy(child.gameObject);

            foreach (ItemDataSO item in InventoryManager.Instance.Items)
            {
                if (rowPrefab == null) break;
                GameObject row = Instantiate(rowPrefab, listParent);
                TMP_Text label = row.GetComponentInChildren<TMP_Text>();
                if (label != null) label.text = item.displayName;
            }

            WeaponData equipped = InventoryManager.Instance.EquippedWeapon;
            if (equipped != null)
            {
                if (equippedWeaponIcon != null) equippedWeaponIcon.sprite = equipped.weaponIcon;
                if (equippedWeaponName != null) equippedWeaponName.text = equipped.weaponName;
            }
        }
    }
}
