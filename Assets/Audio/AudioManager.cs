using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [Tooltip("Assign Assets/Audio/AudioMixer.mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources (auto-created if empty)")]
    public AudioSource musicSource;
    public AudioSource ambientSource;
    public AudioSource uiSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("Optional default clips")]
    public AudioClip musicDemo;
    public AudioClip sfxDemo;
    public AudioClip voiceDemo;
    public AudioClip uiDemo;
    public AudioClip ambientDemo;

    private const string MasterKey = "MasterVolume";
    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SFXVolume";
    private const string VoiceKey = "VoiceVolume";
    private const string UiKey = "UIVolume";
    private const string AmbientKey = "AmbientVolume";
    private const string MuteAllKey = "MuteAll";

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

        EnsureAudioSources();
        LoadVolumes();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (audioMixer == null)
        {
            AudioMixer mixer = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioMixer>(
                "Assets/Audio/AudioMixer.mixer");
            if (mixer != null)
                audioMixer = mixer;
        }
#endif
    }

    private void EnsureAudioSources()
    {
        musicSource = EnsureSource(musicSource, "MusicSource", "Music");
        ambientSource = EnsureSource(ambientSource, "AmbientSource", "Ambient");
        sfxSource = EnsureSource(sfxSource, "SFXSource", "SFX");
        uiSource = EnsureSource(uiSource, "UISource", "UI");
        voiceSource = EnsureSource(voiceSource, "VoiceSource", "Voice");
    }

    private AudioSource EnsureSource(AudioSource existing, string childName, string mixerGroupName)
    {
        if (existing != null)
        {
            AssignMixerGroup(existing, mixerGroupName);
            return existing;
        }

        Transform child = transform.Find(childName);
        if (child != null)
            existing = child.GetComponent<AudioSource>();

        if (existing == null)
        {
            GameObject go = new GameObject(childName);
            go.transform.SetParent(transform, false);
            existing = go.AddComponent<AudioSource>();
            existing.playOnAwake = false;
        }

        AssignMixerGroup(existing, mixerGroupName);
        return existing;
    }

    private void AssignMixerGroup(AudioSource source, string groupName)
    {
        if (source == null || audioMixer == null) return;

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups(groupName);
        if (groups != null && groups.Length > 0)
            source.outputAudioMixerGroup = groups[0];
    }

    public void SetMasterVolume(float value) => SetVolume("MasterVolume", MasterKey, value);
    public void SetMusicVolume(float value) => SetVolume("MusicVolume", MusicKey, value);
    public void SetSFXVolume(float value) => SetVolume("SFXVolume", SfxKey, value);
    public void SetDialogueVolume(float value) => SetVolume("VoiceVolume", VoiceKey, value);
    public void SetVoiceVolume(float value) => SetDialogueVolume(value);
    public void SetUIVolume(float value) => SetVolume("UIVolume", UiKey, value);
    public void SetAmbientVolume(float value) => SetVolume("AmbientVolume", AmbientKey, value);

    private void SetVolume(string exposedParam, string prefKey, float sliderValue)
    {
        PlayerPrefs.SetFloat(prefKey, Mathf.Clamp(sliderValue, 0.0001f, 1f));
        PlayerPrefs.Save();

        if (!isMuted && HasMixer)
            audioMixer.SetFloat(exposedParam, SliderToDb(sliderValue));
    }

    public float GetSavedVolume(string key, float defaultValue = 1f) =>
        PlayerPrefs.GetFloat(key, defaultValue);

    public bool GetMuteState() => PlayerPrefs.GetInt(MuteAllKey, 0) == 1;

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

        SetMixerOnly("MasterVolume", GetSavedVolume(MasterKey));
        SetMixerOnly("MusicVolume", GetSavedVolume(MusicKey));
        SetMixerOnly("SFXVolume", GetSavedVolume(SfxKey));
        SetMixerOnly("VoiceVolume", GetSavedVolume(VoiceKey));
        SetMixerOnly("UIVolume", GetSavedVolume(UiKey));
        SetMixerOnly("AmbientVolume", GetSavedVolume(AmbientKey));
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

    private static float SliderToDb(float sliderValue) =>
        Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;

    private static void LogMixerMissingOnce()
    {
        if (mixerWarningLogged) return;
        mixerWarningLogged = true;
        Debug.LogWarning("[AudioManager] Assign Assets/Audio/AudioMixer.mixer in the inspector.");
    }

    public void MuteAll()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt(MuteAllKey, isMuted ? 1 : 0);
        PlayerPrefs.Save();

        if (!HasMixer) return;

        if (isMuted)
            ApplyMutedMixerState();
        else
            ApplySavedVolumesToMixer();
    }

    public bool IsMuted() => isMuted;

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource == null) return;
        ambientSource.clip = clip;
        ambientSource.loop = true;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource != null)
            ambientSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip == null || uiSource == null) return;
        uiSource.PlayOneShot(clip);
    }

    public void PlayDialogue(AudioClip clip) => PlayVoice(clip);

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
