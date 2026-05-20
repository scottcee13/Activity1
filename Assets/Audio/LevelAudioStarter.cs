using System.Collections;
using UnityEngine;

public class LevelAudioStarter : MonoBehaviour
{
    public AudioClip bgm;
    public AudioClip ambient;

    [SerializeField] private float retrySeconds = 2f;

    private void Start()
    {
        StartCoroutine(PlayWhenReady());
    }

    private IEnumerator PlayWhenReady()
    {
        float waited = 0f;
        while (AudioManager.Instance == null && waited < retrySeconds)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[LevelAudioStarter] AudioManager.Instance is null. Is AudioManager in the scene?");
            yield break;
        }

        if (bgm != null)
        {
            AudioManager.Instance.PlayMusic(bgm);
            Debug.Log("[LevelAudioStarter] Playing BGM.");
        }
        else
        {
            Debug.LogWarning("[LevelAudioStarter] BGM clip not assigned.");
        }

        if (ambient != null)
        {
            AudioManager.Instance.PlayAmbient(ambient);
            Debug.Log("[LevelAudioStarter] Playing ambient.");
        }
    }
}
