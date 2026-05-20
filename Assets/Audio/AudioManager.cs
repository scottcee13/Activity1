using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [Tooltip("Assign Assets/Audio/AudioMixer.mixer — required for volume sliders.")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource uiSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("Demo Clips")]
    public AudioClip musicDemo;
    public AudioClip sfxDemo;
    public AudioClip voiceDemo;
    public AudioClip uiDemo;
    public AudioClip ambientDemo;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string VOICE_KEY = "VoiceVolume";
    private const string UI_KEY = "UIVolume";
    private const string AMBIENT_KEY = "AmbientVolume";
    private const string MUTE_ALL_KEY = "MuteAll";

    private bool isMuted;
    private static bool mixerWarningLogged;

    public bool HasMixer => audioMixer != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadVolumes();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (audioMixer == null && !mixerWarningLogged)
        {
            AudioMixer mixer = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(
                "Assets/Audio/AudioMixer.mixer");
            if (mixer != null)
                audioMixer = mixer;
        }
#endif
    }

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", MASTER_KEY, value);
        if (!isMuted)
            SetMixerOnly("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", MUSIC_KEY, value);
        if (!isMuted)
            SetMixerOnly("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", SFX_KEY, value);
        if (!isMuted)
            SetMixerOnly("SFXVolume", value);
    }

    public void SetVoiceVolume(float value)
    {
        SetVolume("VoiceVolume", VOICE_KEY, value);
        if (!isMuted)
            SetMixerOnly("VoiceVolume", value);
    }

    public void SetUIVolume(float value)
    {
        SetVolume("UIVolume", UI_KEY, value);
        if (!isMuted)
            SetMixerOnly("UIVolume", value);
    }

    public void SetAmbientVolume(float value)
    {
        SetVolume("AmbientVolume", AMBIENT_KEY, value);
        if (!isMuted)
            SetMixerOnly("AmbientVolume", value);
    }

    private void SetVolume(string exposedParam, string prefKey, float sliderValue)
    {
        PlayerPrefs.SetFloat(prefKey, Mathf.Clamp(sliderValue, 0.0001f, 1f));
        PlayerPrefs.Save();

        if (!HasMixer) return;

        float dB = SliderToDb(sliderValue);
        audioMixer.SetFloat(exposedParam, dB);
    }

    public float GetSavedVolume(string key, float defaultValue = 1f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public bool GetMuteState()
    {
        return PlayerPrefs.GetInt(MUTE_ALL_KEY, 0) == 1;
    }

    public void LoadVolumes()
    {
        isMuted = GetMuteState();

        if (!HasMixer)
        {
            LogMixerMissingOnce();
            return;
        }

        if (isMuted)
            ApplyMutedMixerState();
        else
            ApplySavedVolumesToMixer();
    }

    public void ApplySavedVolumesToMixer()
    {
        if (!HasMixer) return;

        SetMixerOnly("MasterVolume", GetSavedVolume(MASTER_KEY));
        SetMixerOnly("MusicVolume", GetSavedVolume(MUSIC_KEY));
        SetMixerOnly("SFXVolume", GetSavedVolume(SFX_KEY));
        SetMixerOnly("VoiceVolume", GetSavedVolume(VOICE_KEY));
        SetMixerOnly("UIVolume", GetSavedVolume(UI_KEY));
        SetMixerOnly("AmbientVolume", GetSavedVolume(AMBIENT_KEY));
    }

    private void ApplyMutedMixerState()
    {
        if (!HasMixer) return;

        SetMixerOnly("MasterVolume", 0.0001f);
        SetMixerOnly("MusicVolume", 0.0001f);
        SetMixerOnly("SFXVolume", 0.0001f);
        SetMixerOnly("VoiceVolume", 0.0001f);
        SetMixerOnly("UIVolume", 0.0001f);
        SetMixerOnly("AmbientVolume", 0.0001f);
    }

    private void SetMixerOnly(string exposedParam, float sliderValue)
    {
        if (!HasMixer) return;
        audioMixer.SetFloat(exposedParam, SliderToDb(sliderValue));
    }

    private static float SliderToDb(float sliderValue)
    {
        return Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
    }

    private static void LogMixerMissingOnce()
    {
        if (mixerWarningLogged) return;
        mixerWarningLogged = true;
        Debug.LogWarning(
            "[AudioManager] Audio Mixer is not assigned. " +
            "Select AudioManager → drag Assets/Audio/AudioMixer.mixer into the Audio Mixer field. " +
            "Volume sliders will save to PlayerPrefs but won't affect the mixer until assigned.");
    }

    public void MuteAll()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MUTE_ALL_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (!HasMixer) return;

        if (isMuted)
            ApplyMutedMixerState();
        else
            ApplySavedVolumesToMixer();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource == null) return;
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public bool IsMuted() => isMuted;

    public void PlayUI(AudioClip clip)
    {
        if (clip == null || uiSource == null) return;
        uiSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null || voiceSource == null) return;
        voiceSource.PlayOneShot(clip);
    }

    public void DemoMusic() => PlayMusic(musicDemo);
    public void DemoSFX() => PlaySFX(sfxDemo);
    public void DemoUI() => PlayUI(uiDemo);
    public void DemoAmbient() => PlayAmbient(ambientDemo);
    public void DemoVoice() => PlayVoice(voiceDemo);
}
