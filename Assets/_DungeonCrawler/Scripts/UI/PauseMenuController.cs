using DungeonCrawler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (AudioManager.Instance == null) return;

            if (masterSlider != null)
            {
                masterSlider.value = AudioManager.Instance.GetSavedVolume("MasterVolume");
                masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
            }
            if (musicSlider != null)
            {
                musicSlider.value = AudioManager.Instance.GetSavedVolume("MusicVolume");
                musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = AudioManager.Instance.GetSavedVolume("SFXVolume");
                sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
            }

            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => UIManager.Instance?.TogglePause());
            if (restartButton != null)
                restartButton.onClick.AddListener(() => GameManager.Instance?.RestartDungeon());
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => SceneFlowManager.Instance?.LoadMainMenu());
        }
    }
}
