using DungeonCrawler.Quests;
using DungeonCrawler.UI;
using UnityEngine;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Scene bootstrap: validates managers, UI singletons, and audio. Add to _GAME_MANAGERS.
    /// </summary>
    public class GameSystemsBootstrap : MonoBehaviour
    {
        [Header("Optional UI (auto-find by name if empty)")]
        [SerializeField] private ControlPromptUI controlPromptUI;
        [SerializeField] private GameObject questTrackerPanel;

        [Header("Debug")]
        [SerializeField] private bool logValidation = true;

        private void Awake()
        {
            if (GetComponent<QuestEventBridge>() == null)
                gameObject.AddComponent<QuestEventBridge>();

            ValidateQuestManager();
            ValidateControlPromptUI();
            ValidateQuestTracker();
        }

        private void ValidateQuestManager()
        {
            if (QuestManager.Instance != null) return;
            QuestManager found = FindFirstObjectByType<QuestManager>();
            if (found != null && logValidation)
                Debug.Log("[Bootstrap] QuestManager found in scene.");
            else if (logValidation)
                Debug.LogWarning("[Bootstrap] QuestManager missing! Add QuestManager to _GAME_MANAGERS with quest SOs assigned.");
        }

        private void ValidateControlPromptUI()
        {
            if (controlPromptUI == null)
                controlPromptUI = FindFirstObjectByType<ControlPromptUI>();

            if (controlPromptUI == null)
            {
                if (logValidation)
                    Debug.LogWarning("[Bootstrap] ControlPromptUI missing! Create UI panel with ControlPromptUI script (see SETUP_GUIDE).");
                return;
            }

            if (logValidation)
                Debug.Log("[Bootstrap] ControlPromptUI ready.");
        }

        private void ValidateQuestTracker()
        {
            if (questTrackerPanel == null)
            {
                QuestUIManager ui = FindFirstObjectByType<QuestUIManager>();
                if (ui != null && ui.questListParent != null)
                    questTrackerPanel = ui.questListParent.gameObject;
            }

            if (questTrackerPanel != null && !questTrackerPanel.activeInHierarchy)
            {
                questTrackerPanel.SetActive(true);
                if (logValidation)
                    Debug.Log("[Bootstrap] Activated quest tracker panel.");
            }
        }
    }
}
