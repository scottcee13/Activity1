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
        GameOverPanel.SetActive(true);
    }
}
