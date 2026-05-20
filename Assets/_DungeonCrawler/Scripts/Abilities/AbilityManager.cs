using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Abilities
{
    /// <summary>
    /// Registers abilities and routes input. Fires cooldown events for HUD.
    /// </summary>
    public class AbilityManager : MonoBehaviour
    {
        public static AbilityManager Instance { get; private set; }

        [SerializeField] private List<AbilityBase> abilities = new List<AbilityBase>();

        private readonly Dictionary<KeyCode, AbilityBase> keyMap = new Dictionary<KeyCode, AbilityBase>();

        private void Awake()
        {
            Instance = this;
            foreach (AbilityBase ability in abilities)
            {
                if (ability == null || ability.Data == null) continue;
                if (ability.Data.defaultKey != KeyCode.None)
                    keyMap[ability.Data.defaultKey] = ability;
            }
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.IsPaused) return;

            foreach (var pair in keyMap)
            {
                if (Input.GetKeyDown(pair.Key))
                    pair.Value.TryActivate();
            }

            // Space = jump fallback
            if (Input.GetKeyDown(KeyCode.Space))
                TryActivateById("jump");

            if (Input.GetKeyDown(KeyCode.Q))
                TryActivateById("dash");

            if (Input.GetKeyDown(KeyCode.F))
                TryActivateById("projectile");
        }

        public bool TryActivateById(string abilityId)
        {
            foreach (AbilityBase a in abilities)
            {
                if (a != null && a.AbilityId == abilityId)
                    return a.TryActivate();
            }
            return false;
        }

        public IReadOnlyList<AbilityBase> GetAbilities() => abilities;

        public void RegisterAbility(AbilityBase ability)
        {
            if (ability != null && !abilities.Contains(ability))
                abilities.Add(ability);
        }
    }
}
