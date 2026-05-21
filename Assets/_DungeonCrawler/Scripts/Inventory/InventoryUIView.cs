using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.Inventory
{
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

            for (int i = listParent.childCount - 1; i >= 0; i--)
                Destroy(listParent.GetChild(i).gameObject);

            foreach (ItemDataSO item in InventoryManager.Instance.Items)
            {
                if (rowPrefab == null) break;

                GameObject row = Instantiate(rowPrefab, listParent);

                InventoryItemRowUI rowUi = row.GetComponent<InventoryItemRowUI>();
                if (rowUi != null)
                {
                    rowUi.Bind(item);
                    continue;
                }

                Image icon = row.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = item.icon;
                    icon.enabled = item.icon != null;
                    icon.preserveAspect = true;
                    icon.color = item.icon != null ? Color.white : new Color(1f, 1f, 1f, 0.3f);
                }

                TMP_Text label = row.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = item.displayName;
            }

            if (listParent is RectTransform rt)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

            WeaponData equipped = InventoryManager.Instance.EquippedWeapon;
            if (equipped != null)
            {
                if (equippedWeaponIcon != null)
                {
                    equippedWeaponIcon.sprite = equipped.weaponIcon;
                    equippedWeaponIcon.enabled = equipped.weaponIcon != null;
                }
                if (equippedWeaponName != null)
                    equippedWeaponName.text = equipped.weaponName;
            }
        }
    }
}
