using TMPro;
using UnityEngine;

namespace DungeonCrawler.UI
{
    /// <summary>
    /// Shared "Press E" prompt for NPCs and pickups. One instance on Canvas.
    /// </summary>
    public class InteractPromptUI : MonoBehaviour
    {
        public static InteractPromptUI Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private string defaultMessage = "Press E to interact";

        private int showCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            Hide();
        }

        public void Show(string message = null)
        {
            showCount++;
            if (panel != null) panel.SetActive(true);
            if (messageText != null)
                messageText.text = string.IsNullOrEmpty(message) ? defaultMessage : message;
        }

        public void Hide()
        {
            showCount = Mathf.Max(0, showCount - 1);
            if (showCount > 0) return;
            if (panel != null) panel.SetActive(false);
        }

        public void ForceHide()
        {
            showCount = 0;
            if (panel != null) panel.SetActive(false);
        }
    }
}
