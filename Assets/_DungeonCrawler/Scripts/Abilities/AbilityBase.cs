using System;
using UnityEngine;

namespace DungeonCrawler.Abilities
{
    /// <summary>
    /// Base class for player abilities. Handles cooldown timing and events.
    /// </summary>
    public abstract class AbilityBase : MonoBehaviour
    {
        [SerializeField] protected AbilityDataSO data;

        public string AbilityId => data != null ? data.abilityId : name;
        public AbilityDataSO Data => data;
        public float CooldownRemaining { get; private set; }
        public bool IsOnCooldown => CooldownRemaining > 0f;

        public event Action<AbilityBase, float> OnCooldownUpdated;

        protected virtual void Update()
        {
            if (CooldownRemaining > 0f)
            {
                CooldownRemaining -= Time.deltaTime;
                if (CooldownRemaining < 0f) CooldownRemaining = 0f;
                OnCooldownUpdated?.Invoke(this, GetCooldownNormalized());
            }
        }

        public float GetCooldownNormalized()
        {
            if (data == null || data.cooldown <= 0f) return 0f;
            return CooldownRemaining / data.cooldown;
        }

        public bool TryActivate()
        {
            if (IsOnCooldown || data == null) return false;
            if (!CanExecute()) return false;

            Execute();
            CooldownRemaining = data.cooldown;
            OnCooldownUpdated?.Invoke(this, 1f);
            Core.GameEvents.RaiseAbilityUsed(AbilityId);
            return true;
        }

        protected abstract bool CanExecute();
        protected abstract void Execute();
    }
}
