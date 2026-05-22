using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Linear quest chain: only the active quest receives progress. Attach to _GAME_MANAGERS.
    /// </summary>
    public class QuestChainController : MonoBehaviour
    {
        public static QuestChainController Instance { get; private set; }

        [SerializeField] private List<string> questIdsInOrder = new List<string>
        {
            "tutorial_equip_weapon",
            "tutorial_talk",
            "tutorial_move",
            "tutorial_combat",
            "puzzle_key",
            "boss_slain"
        };

        [SerializeField] private bool logSteps = true;

        public event Action OnActiveQuestChanged;

        private int currentIndex;

        public string ActiveQuestId =>
            currentIndex >= 0 && currentIndex < questIdsInOrder.Count
                ? questIdsInOrder[currentIndex]
                : null;

        public bool IsChainComplete => currentIndex >= questIdsInOrder.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnQuestUpdated += OnQuestUpdated;
        }

        private void OnDisable()
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnQuestUpdated -= OnQuestUpdated;
        }

        private void Start()
        {
            SyncChainIndexFromQuests();
            if (logSteps)
                Debug.Log($"[QuestChain] Active quest: {ActiveQuestId ?? "(complete)"}");
        }

        public bool IsQuestActive(string questId) =>
            !string.IsNullOrEmpty(questId) && ActiveQuestId == questId;

        public bool TryAdvanceActiveQuest(int amount = 1)
        {
            string id = ActiveQuestId;
            if (string.IsNullOrEmpty(id) || QuestManager.Instance == null) return false;
            return QuestManager.Instance.AdvanceQuest(id, amount);
        }

        public void LoadChainIndex(int index)
        {
            currentIndex = Mathf.Clamp(index, 0, questIdsInOrder.Count);
        }

        public int GetChainIndexForSave() => currentIndex;

        public void SyncChainIndexFromQuests()
        {
            if (QuestManager.Instance == null) return;

            for (int i = 0; i < questIdsInOrder.Count; i++)
            {
                QuestInstance quest = QuestManager.Instance.GetQuest(questIdsInOrder[i]);
                if (quest == null || !quest.status.isCompleted)
                {
                    currentIndex = i;
                    return;
                }
            }

            currentIndex = questIdsInOrder.Count;
        }

        private void OnQuestUpdated()
        {
            if (IsChainComplete || QuestManager.Instance == null) return;

            QuestInstance active = QuestManager.Instance.GetQuest(ActiveQuestId);
            if (active == null || !active.status.isCompleted) return;

            if (logSteps)
                Debug.Log($"[QuestChain] Completed: {ActiveQuestId}");

            currentIndex++;

            if (logSteps && !IsChainComplete)
                Debug.Log($"[QuestChain] Next: {ActiveQuestId}");
            else if (logSteps && IsChainComplete)
                Debug.Log("[QuestChain] All quests complete.");

            QuestManager.Instance?.NotifyQuestUpdated();
            OnActiveQuestChanged?.Invoke();

            if (QuestUIManager.Instance != null)
                QuestUIManager.Instance.RefreshUI();
        }
    }
}
