using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
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

    private bool isMuted = false;

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
    private void SaveVolume(string prefKey, float sliderValue)
    {
        PlayerPrefs.SetFloat(prefKey, Mathf.Clamp(sliderValue, 0.0001f, 1f));
        PlayerPrefs.Save();
    }
    private void SetVolume(string exposedParam, string prefKey, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
        PlayerPrefs.SetFloat(prefKey, sliderValue);
        PlayerPrefs.Save();
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

        if (isMuted)
        {
            ApplyMutedMixerState();
        }
        else
        {
            ApplySavedVolumesToMixer();
        }
    }

    public void ApplySavedVolumesToMixer()
    {
        SetMixerOnly("MasterVolume", GetSavedVolume(MASTER_KEY));
        SetMixerOnly("MusicVolume", GetSavedVolume(MUSIC_KEY));
        SetMixerOnly("SFXVolume", GetSavedVolume(SFX_KEY));
        SetMixerOnly("VoiceVolume", GetSavedVolume(VOICE_KEY));
        SetMixerOnly("UIVolume", GetSavedVolume(UI_KEY));
        SetMixerOnly("AmbientVolume", GetSavedVolume(AMBIENT_KEY));
    }

    private void ApplyMutedMixerState()
    {
        SetMixerOnly("MasterVolume", 0.0001f);
        SetMixerOnly("MusicVolume", 0.0001f);
        SetMixerOnly("SFXVolume", 0.0001f);
        SetMixerOnly("VoiceVolume", 0.0001f);
        SetMixerOnly("UIVolume", 0.0001f);
        SetMixerOnly("AmbientVolume", 0.0001f);
    }

    private void SetMixerOnly(string exposedParam, float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(exposedParam, dB);
    }

    public void MuteAll()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MUTE_ALL_KEY, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (isMuted)
            ApplyMutedMixerState();
        else
            ApplySavedVolumesToMixer();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public bool IsMuted()
    {
        return isMuted;
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null) return;
        uiSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null) return;
        voiceSource.PlayOneShot(clip);
    }

    public void DemoMusic()
    {
        PlayMusic(musicDemo);
    }

    public void DemoSFX()
    {
        PlaySFX(sfxDemo);
    }

    public void DemoVoice(AudioSource source)
    {
        if (voiceDemo == null || source == null) return;
        source.PlayOneShot(voiceDemo);
    }

    public void DemoUI()
    {
        PlayUI(uiDemo);
    }

    public void DemoAmbient()
    {
        PlayAmbient(ambientDemo);
    }

    public void DemoVoice()
    {
        PlayVoice(voiceDemo);
    }
}