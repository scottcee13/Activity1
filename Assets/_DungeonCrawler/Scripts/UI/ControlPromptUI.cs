using System.Collections;
using TMPro;
using UnityEngine;

namespace DungeonCrawler.UI
{
  /// <summary>
  /// Tutorial / control hint overlay. Must exist once in scene Canvas.
  /// </summary>
    public class ControlPromptUI : MonoBehaviour
    {
        public static ControlPromptUI Instance { get; private set; }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text messageText;

        private Coroutine hideRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[ControlPromptUI] Duplicate instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;

            if (panel == null)
                panel = gameObject;

            if (messageText == null)
                messageText = GetComponentInChildren<TMP_Text>(true);

            if (panel != null)
                panel.SetActive(false);
        }

        public void Show(string message, float duration = 4f)
        {
            if (messageText != null)
                messageText.text = message;
            else
                Debug.LogWarning("[ControlPromptUI] messageText not assigned!");

            if (panel != null)
                panel.SetActive(true);
            else
            {
                Debug.Log($"[ControlPrompt] {message}");
                return;
            }

            if (hideRoutine != null)
                StopCoroutine(hideRoutine);
            hideRoutine = StartCoroutine(HideAfter(duration));

            Debug.Log($"[ControlPromptUI] Showing: {message}");
        }

        private IEnumerator HideAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (panel != null)
                panel.SetActive(false);
        }
    }
}
