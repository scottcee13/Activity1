using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Top-level game state: pause, victory, game over. Coordinates cursor and time scale.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private string playerTag = "Player";

        public bool IsPaused { get; private set; }
        public bool IsVictory { get; private set; }
        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerDied += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerDied -= HandlePlayerDeath;
        }

        private void Start()
        {
            if (player == null)
            {
                GameObject found = GameObject.FindGameObjectWithTag(playerTag);
                if (found != null) player = found.transform;
            }
        }

        public Transform GetPlayer() => player;

        public void SetGameplayPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            GameEvents.RaiseGameplayPauseChanged(paused);
        }

        public void TogglePause()
        {
            SetGameplayPaused(!IsPaused);
        }

        public void TriggerVictory()
        {
            if (IsVictory) return;
            IsVictory = true;
            SetGameplayPaused(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameEvents.RaiseVictory();
        }

        private void HandlePlayerDeath()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            SetGameplayPaused(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void RestartDungeon()
        {
            Time.timeScale = 1f;
            IsVictory = false;
            IsGameOver = false;
            IsPaused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
