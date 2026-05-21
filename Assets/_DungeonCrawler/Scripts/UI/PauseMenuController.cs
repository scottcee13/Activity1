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
        [SerializeField] private Slider dialogueSlider;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider uiSlider;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Start()
        {
            if (AudioManager.Instance == null) return;

            BindSlider(masterSlider, "MasterVolume", AudioManager.Instance.SetMasterVolume);
            BindSlider(musicSlider, "MusicVolume", AudioManager.Instance.SetMusicVolume);
            BindSlider(sfxSlider, "SFXVolume", AudioManager.Instance.SetSFXVolume);
            BindSlider(dialogueSlider, "VoiceVolume", AudioManager.Instance.SetDialogueVolume);
            BindSlider(ambientSlider, "AmbientVolume", AudioManager.Instance.SetAmbientVolume);
            BindSlider(uiSlider, "UIVolume", AudioManager.Instance.SetUIVolume);

            if (resumeButton != null)
                resumeButton.onClick.AddListener(() => UIManager.Instance?.TogglePause());
            if (restartButton != null)
                restartButton.onClick.AddListener(() => GameManager.Instance?.RestartDungeon());
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => SceneFlowManager.Instance?.LoadMainMenu());
        }

        private static void BindSlider(Slider slider, string prefKey, UnityEngine.Events.UnityAction<float> setter)
        {
            if (slider == null) return;
            slider.SetValueWithoutNotify(AudioManager.Instance.GetSavedVolume(prefKey));
            slider.onValueChanged.AddListener(setter);
        }
    }
}
