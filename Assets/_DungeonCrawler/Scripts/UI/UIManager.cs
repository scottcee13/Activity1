using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private GameObject hudRoot;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;

        private void Awake()
        {
            Instance = this;

            if (gameOverPanel == null)
            {
                GameObject found = GameObject.Find("GameOverUI");
                if (found != null) gameOverPanel = found;
            }

            if (victoryPanel == null)
            {
                GameObject found = GameObject.Find("VictoryPanel");
                if (found != null) victoryPanel = found;
            }

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (victoryPanel != null)
                victoryPanel.SetActive(false);

            EnsureVictoryScreenUI();
            EnsureGameOverScreenUI();
        }

        private void Update()
        {
            if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsVictory))
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();

            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleInventory();
        }

        public void TogglePause()
        {
            bool show = pauseMenu != null && !pauseMenu.activeSelf;
            if (pauseMenu != null) pauseMenu.SetActive(show);

            GameManager.Instance?.SetGameplayPaused(show);

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
            if (pauseMenu != null) pauseMenu.SetActive(false);
            if (inventoryPanel != null) inventoryPanel.SetActive(false);

            if (victoryPanel != null)
                victoryPanel.SetActive(true);

            VictoryScreenUI victory = victoryPanel != null
                ? victoryPanel.GetComponent<VictoryScreenUI>()
                : null;

            if (victory == null && victoryPanel != null)
                victory = victoryPanel.AddComponent<VictoryScreenUI>();

            victory?.Populate();
        }

        public void ShowGameOver()
        {
            if (pauseMenu != null) pauseMenu.SetActive(false);
            if (inventoryPanel != null) inventoryPanel.SetActive(false);

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            GameOverScreenUI gameOver = gameOverPanel != null
                ? gameOverPanel.GetComponent<GameOverScreenUI>()
                : null;

            if (gameOver == null && gameOverPanel != null)
                gameOver = gameOverPanel.AddComponent<GameOverScreenUI>();

            gameOver?.Show();
        }

        private void OnEnable()
        {
            GameEvents.OnVictory += HandleVictory;
            GameEvents.OnPlayerDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            GameEvents.OnVictory -= HandleVictory;
            GameEvents.OnPlayerDied -= HandlePlayerDied;
        }

        private void HandleVictory() => ShowVictory();

        private void HandlePlayerDied() => ShowGameOver();

        private void EnsureVictoryScreenUI()
        {
            if (victoryPanel == null) return;
            if (victoryPanel.GetComponent<VictoryScreenUI>() == null)
                victoryPanel.AddComponent<VictoryScreenUI>();
        }

        private void EnsureGameOverScreenUI()
        {
            if (gameOverPanel == null) return;
            if (gameOverPanel.GetComponent<GameOverScreenUI>() == null)
                gameOverPanel.AddComponent<GameOverScreenUI>();
        }
    }
}
