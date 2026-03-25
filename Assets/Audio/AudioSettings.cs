using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider voiceSlider;
    public Slider uiSlider;
    public Slider ambientSlider;

    private bool isInitializing = false;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager instance not found in scene.");
            return;
        }

        isInitializing = true;

        masterSlider.minValue = 0.0001f;
        musicSlider.minValue = 0.0001f;
        sfxSlider.minValue = 0.0001f;
        voiceSlider.minValue = 0.0001f;
        uiSlider.minValue = 0.0001f;
        ambientSlider.minValue = 0.0001f;

        masterSlider.maxValue = 1f;
        musicSlider.maxValue = 1f;
        sfxSlider.maxValue = 1f;
        voiceSlider.maxValue = 1f;
        uiSlider.maxValue = 1f;
        ambientSlider.maxValue = 1f;

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        voiceSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1f);
        uiSlider.value = PlayerPrefs.GetFloat("UIVolume", 1f);
        ambientSlider.value = PlayerPrefs.GetFloat("AmbientVolume", 1f);

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        voiceSlider.onValueChanged.AddListener(OnVoiceChanged);
        uiSlider.onValueChanged.AddListener(OnUIChanged);
        ambientSlider.onValueChanged.AddListener(OnAmbientChanged);

        isInitializing = false;

        AudioManager.Instance.LoadVolumes();
    }

    private void OnMasterChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnVoiceChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetVoiceVolume(value);
    }

    private void OnUIChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetUIVolume(value);
    }

    private void OnAmbientChanged(float value)
    {
        if (isInitializing) return;
        AudioManager.Instance.SetAmbientVolume(value);
    }

    public void MuteAll()
    {
        AudioManager.Instance.MuteAll();
    }

    public void DemoMusic() => AudioManager.Instance.DemoMusic();
    public void DemoSFX() => AudioManager.Instance.DemoSFX();
    public void DemoUI() => AudioManager.Instance.DemoUI();
    public void DemoAmbient() => AudioManager.Instance.DemoAmbient();
    public void DemoVoice() => AudioManager.Instance.DemoVoice();
}