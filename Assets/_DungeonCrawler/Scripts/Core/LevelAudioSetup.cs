using UnityEngine;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Drag BGM and Ambient clips here. Add to _GAME_MANAGERS or any scene object.
    /// </summary>
    public class LevelAudioSetup : MonoBehaviour
    {
        [Header("Music — assign clips here")]
        public AudioClip backgroundMusic;
        public AudioClip ambientLoop;

        [Header("Optional one-shots at level start")]
        public AudioClip introVoice;

        private void Start()
        {
            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("[LevelAudioSetup] AudioManager missing from scene.");
                return;
            }

            if (backgroundMusic != null)
                AudioManager.Instance.PlayMusic(backgroundMusic);

            if (ambientLoop != null)
                AudioManager.Instance.PlayAmbient(ambientLoop);

            if (introVoice != null)
                AudioManager.Instance.PlayDialogue(introVoice);
        }
    }
}
