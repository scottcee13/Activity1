using DungeonCrawler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject settingsPanel;

        private void Start()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playButton != null)
                playButton.onClick.AddListener(OnPlay);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);
            if (settingsButton != null)
                settingsButton.onClick.AddListener(() =>
                {
                    if (settingsPanel != null) settingsPanel.SetActive(!settingsPanel.activeSelf);
                });
        }

        private void OnPlay()
        {
            if (SceneFlowManager.Instance != null)
                SceneFlowManager.Instance.LoadDungeon();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        private void OnQuit()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.QuitGame();
            else
                Application.Quit();
        }
    }
}
