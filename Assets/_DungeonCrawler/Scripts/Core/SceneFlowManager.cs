using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Loads main menu and dungeon scenes. Extend for additive room loading.
    /// </summary>
    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }

        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string dungeonSceneName = "DungeonCrawler";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }

        public void LoadDungeon()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(dungeonSceneName);
        }

        public void LoadSceneByName(string sceneName)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}
