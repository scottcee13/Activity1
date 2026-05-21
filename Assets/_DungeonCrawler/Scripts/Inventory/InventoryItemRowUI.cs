using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.Inventory
{
    /// <summary>
    /// Binds ItemDataSO to a grid row (icon + name). Add to GridRowPrefab root.
    /// </summary>
    public class InventoryItemRowUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;

        public void Bind(ItemDataSO item)
        {
            if (item == null) return;

            if (iconImage == null)
                iconImage = GetComponent<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = item.icon;
                iconImage.enabled = item.icon != null;
                iconImage.color = item.icon != null ? Color.white : new Color(1f, 1f, 1f, 0.25f);
                iconImage.preserveAspect = true;
            }

            if (nameText == null)
                nameText = GetComponentInChildren<TMP_Text>();

            if (nameText != null)
                nameText.text = item.displayName;
        }
    }
}
