using UnityEngine;

public class LevelAudioStarter : MonoBehaviour
{
    public AudioClip bgm;
    public AudioClip ambient;

    void Start()
    {
        AudioManager.Instance.PlayMusic(bgm);
        AudioManager.Instance.PlayAmbient(ambient);
    }
}