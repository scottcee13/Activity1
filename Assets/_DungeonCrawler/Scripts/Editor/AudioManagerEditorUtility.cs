#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace DungeonCrawler.Editor
{
    public static class AudioManagerEditorUtility
    {
        [MenuItem("Tools/Dungeon/Fix AudioManager Mixer Reference")]
        private static void AssignMixerToAllAudioManagers()
        {
            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Audio/AudioMixer.mixer");
            if (mixer == null)
            {
                Debug.LogError("Could not find Assets/Audio/AudioMixer.mixer");
                return;
            }

            int count = 0;
            foreach (AudioManager manager in Object.FindObjectsByType<AudioManager>(FindObjectsSortMode.None))
            {
                if (manager.audioMixer == mixer) continue;
                Undo.RecordObject(manager, "Assign Audio Mixer");
                manager.audioMixer = mixer;
                EditorUtility.SetDirty(manager);
                count++;
            }

            if (count > 0)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log($"[Dungeon] Assigned AudioMixer to {count} AudioManager(s). Save the scene (Ctrl+S).");
        }
    }
}
#endif
