using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Completes tutorial_equip_weapon when the player equips a weapon (Press E).
    /// </summary>
    public class EquipWeaponQuestBridge : MonoBehaviour
    {
        public const string QuestId = "tutorial_equip_weapon";

        [SerializeField] private bool logCompletion = true;
        [SerializeField] private bool onlyOnce = true;

        private bool completed;

        private void OnEnable()
        {
            GameEvents.OnWeaponEquipped += OnWeaponEquipped;
        }

        private void OnDisable()
        {
            GameEvents.OnWeaponEquipped -= OnWeaponEquipped;
        }

        private void OnWeaponEquipped(WeaponData weapon)
        {
            if (weapon == null) return;
            if (onlyOnce && completed) return;

            if (QuestChainController.Instance != null && !QuestChainController.Instance.IsQuestActive(QuestId))
                return;

            if (QuestManager.Instance == null) return;

            QuestInstance quest = QuestManager.Instance.GetQuest(QuestId);
            if (quest == null || quest.status.isCompleted) return;

            QuestManager.Instance.AdvanceQuest(QuestId, 1);
            completed = true;

            if (logCompletion)
                Debug.Log("[Quest] Equip weapon tutorial complete.");
        }
    }
}
