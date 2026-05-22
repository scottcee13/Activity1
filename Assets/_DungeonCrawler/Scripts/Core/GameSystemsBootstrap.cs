using DungeonCrawler.Quests;
using DungeonCrawler.UI;
using UnityEngine;

namespace DungeonCrawler.Core
{
    public class GameSystemsBootstrap : MonoBehaviour
    {
        [Header("Tutorial equip quest")]
        [SerializeField] private QuestDataSO tutorialEquipWeaponQuest;

        [Header("Optional UI (auto-find if empty)")]
        [SerializeField] private ControlPromptUI controlPromptUI;
        [SerializeField] private GameObject questTrackerPanel;

        [Header("Debug")]
        [SerializeField] private bool logValidation = true;

        private void Awake()
        {
            if (GetComponent<QuestEventBridge>() == null)
                gameObject.AddComponent<QuestEventBridge>();

            if (GetComponent<QuestChainController>() == null)
                gameObject.AddComponent<QuestChainController>();

            if (GetComponent<EquipWeaponQuestBridge>() == null)
                gameObject.AddComponent<EquipWeaponQuestBridge>();

            RegisterTutorialEquipQuest();
            ValidateQuestManager();
            ValidateControlPromptUI();
            ValidateQuestTracker();
        }

        private void Start()
        {
            RegisterTutorialEquipQuest();
        }

        private void RegisterTutorialEquipQuest()
        {
            if (tutorialEquipWeaponQuest == null)
                tutorialEquipWeaponQuest = Resources.Load<QuestDataSO>("TutorialEquipWeapon");

#if UNITY_EDITOR
            if (tutorialEquipWeaponQuest == null)
                tutorialEquipWeaponQuest = UnityEditor.AssetDatabase.LoadAssetAtPath<QuestDataSO>(
                    "Assets/_DungeonCrawler/ScriptableObjects/Quests/TutorialEquipWeapon.asset");
#endif

            if (QuestManager.Instance == null || tutorialEquipWeaponQuest == null) return;

            QuestManager.Instance.RegisterQuest(tutorialEquipWeaponQuest);

            if (logValidation)
                Debug.Log("[Bootstrap] Registered tutorial equip weapon quest.");
        }

        private void ValidateQuestManager()
        {
            if (QuestManager.Instance != null) return;
            QuestManager found = FindFirstObjectByType<QuestManager>();
            if (found != null && logValidation)
                Debug.Log("[Bootstrap] QuestManager found in scene.");
            else if (logValidation)
                Debug.LogWarning("[Bootstrap] QuestManager missing on _GAME_MANAGERS.");
        }

        private void ValidateControlPromptUI()
        {
            if (controlPromptUI == null)
                controlPromptUI = FindFirstObjectByType<ControlPromptUI>();

            if (controlPromptUI == null && logValidation)
                Debug.LogWarning("[Bootstrap] ControlPromptUI missing.");
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
                questTrackerPanel.SetActive(true);
        }
    }
}
