using UnityEngine;

public class GameOver : MonoBehaviour
{

    public GameObject GameOverPanel;

    private void Start()
    {
        GameOverPanel.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += GameOverUI;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= GameOverUI;
    }

    private void GameOverUI()
    {
        if (GameOverPanel != null)
            GameOverPanel.SetActive(true);

        if (DungeonCrawler.UI.UIManager.Instance != null)
            DungeonCrawler.UI.UIManager.Instance.ShowGameOver();
    }
}
