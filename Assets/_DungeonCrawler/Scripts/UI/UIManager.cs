using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.UI
{
    /// <summary>
    /// Central UI layer visibility: HUD, pause, inventory, dialogue host.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameObject hudRoot;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject victoryPanel;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();

            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleInventory();
        }

        public void TogglePause()
        {
            bool show = pauseMenu != null && !pauseMenu.activeSelf;
            if (pauseMenu != null) pauseMenu.SetActive(show);

            if (GameManager.Instance != null)
                GameManager.Instance.SetGameplayPaused(show);

            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }

        public void ToggleInventory()
        {
            if (inventoryPanel == null) return;
            bool show = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(show);
            Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = show;
        }

        public void ShowVictory()
        {
            if (victoryPanel != null) victoryPanel.SetActive(true);
            if (hudRoot != null) hudRoot.SetActive(true);
        }

        private void OnEnable()
        {
            GameEvents.OnVictory += HandleVictory;
        }

        private void OnDisable()
        {
            GameEvents.OnVictory -= HandleVictory;
        }

        private void HandleVictory()
        {
            ShowVictory();
            VictoryScreenUI victory = victoryPanel != null
                ? victoryPanel.GetComponent<VictoryScreenUI>()
                : null;
            victory?.Populate();
        }
    }
}
