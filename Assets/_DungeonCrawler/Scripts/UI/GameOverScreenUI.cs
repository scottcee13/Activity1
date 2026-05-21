using DungeonCrawler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class GameOverScreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            AutoFindReferences();
            gameObject.SetActive(false);
            WireButtons();
        }

        private void AutoFindReferences()
        {
            if (messageText == null)
                messageText = GetComponentInChildren<TMP_Text>(true);

            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                string n = btn.name.ToLowerInvariant();
                if (restartButton == null && (n.Contains("restart") || n.Contains("retry") || n.Contains("play")))
                    restartButton = btn;
                if (mainMenuButton == null && (n.Contains("main") || n.Contains("menu") || n.Contains("quit")))
                    mainMenuButton = btn;
            }
        }

        private void WireButtons()
        {
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestart);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(OnMainMenu);
            }
        }

        public void Show()
        {
            AutoFindReferences();
            WireButtons();

            if (messageText != null)
                messageText.text = "You died. Try again?";

            gameObject.SetActive(true);
        }

        private void OnRestart()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.RestartDungeon();
        }

        private void OnMainMenu()
        {
            Time.timeScale = 1f;
            SceneFlowManager.Instance?.LoadMainMenu();
        }
    }
}
