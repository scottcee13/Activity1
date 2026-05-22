using System;
using UnityEngine;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Central event bus. Systems publish here; bridges (quests, UI) subscribe.
    /// Avoids tight coupling between combat, inventory, dialogue, and quests.
    /// </summary>
    public static class GameEvents
    {
        public static event Action<string> OnEnemyKilled;
        public static event Action<string> OnItemCollected;
        public static event Action<WeaponData> OnWeaponEquipped;
        public static event Action OnWeaponUnequipped;
        public static event Action<string> OnDialogueStarted;
        public static event Action<string> OnDialogueEnded;
        public static event Action<string, int> OnDialogueChoiceSelected;
        public static event Action<string> OnQuestObjectiveTriggered;
        public static event Action<string> OnAbilityUsed;
        public static event Action<string> OnRoomEntered;
        public static event Action OnPlayerDied;
        public static event Action OnVictory;
        public static event Action<int> OnPlayerHealthChanged;
        public static event Action<bool> OnGameplayPauseChanged;

        public static void RaiseEnemyKilled(string enemyId) => OnEnemyKilled?.Invoke(enemyId);
        public static void RaiseItemCollected(string itemId) => OnItemCollected?.Invoke(itemId);
        public static void RaiseWeaponEquipped(WeaponData weapon) => OnWeaponEquipped?.Invoke(weapon);
        public static void RaiseWeaponUnequipped() => OnWeaponUnequipped?.Invoke();
        public static void RaiseDialogueStarted(string dialogueId) => OnDialogueStarted?.Invoke(dialogueId);
        public static void RaiseDialogueEnded(string dialogueId) => OnDialogueEnded?.Invoke(dialogueId);
        public static void RaiseDialogueChoice(string dialogueId, int choiceIndex) =>
            OnDialogueChoiceSelected?.Invoke(dialogueId, choiceIndex);
        public static void RaiseQuestObjective(string objectiveId) => OnQuestObjectiveTriggered?.Invoke(objectiveId);

        /// <summary>Alias for RaiseQuestObjective (same event).</summary>
        public static void RaiseQuestObjectiveTriggered(string objectiveId) => RaiseQuestObjective(objectiveId);
        public static void RaiseAbilityUsed(string abilityId) => OnAbilityUsed?.Invoke(abilityId);
        public static void RaiseRoomEntered(string roomId) => OnRoomEntered?.Invoke(roomId);
        public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
        public static void RaiseVictory() => OnVictory?.Invoke();
        public static void RaisePlayerHealthChanged(int current) => OnPlayerHealthChanged?.Invoke(current);
        public static void RaiseGameplayPauseChanged(bool paused) => OnGameplayPauseChanged?.Invoke(paused);
    }
}
