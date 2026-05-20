using System.Collections;
using System.Collections.Generic;
using DungeonCrawler.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonCrawler.UI
{
    /// <summary>
    /// Shows ability icons and cooldown overlays. abilityId on each slot must match AbilityDataSO.abilityId exactly.
    /// </summary>
    public class AbilityCooldownHUD : MonoBehaviour
    {
        [System.Serializable]
        public class AbilitySlotUI
        {
            [Tooltip("Must match Ability Data SO: dash, jump, projectile")]
            public string abilityId;
            public Image icon;
            [Tooltip("Filled Image on top of icon — Type: Filled, Method: Radial 360 or Vertical")]
            public Image cooldownOverlay;
            [Tooltip("Optional — shows key hint e.g. Q")]
            public TMP_Text keyHintText;
        }

        [SerializeField] private List<AbilitySlotUI> slots = new List<AbilitySlotUI>();
        [SerializeField] private bool logBinding = true;

        private void Start()
        {
            StartCoroutine(BindWhenReady());
        }

        private IEnumerator BindWhenReady()
        {
            float timeout = 3f;
            while (AbilityManager.Instance == null && timeout > 0f)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            if (AbilityManager.Instance == null)
            {
                Debug.LogWarning(
                    "[AbilityCooldownHUD] AbilityManager not found. Add AbilityManager to the Player with Dash/Jump/Projectile abilities listed.");
                yield break;
            }

            BindAllAbilities();
        }

        public void BindAllAbilities()
        {
            if (AbilityManager.Instance == null) return;

            foreach (AbilityBase ability in AbilityManager.Instance.GetAbilities())
            {
                if (ability == null || ability.Data == null)
                {
                    Debug.LogWarning($"[AbilityCooldownHUD] Ability '{ability?.name}' has no Ability Data SO assigned on the Player.");
                    continue;
                }

                string id = ability.AbilityId;
                AbilitySlotUI slot = slots.Find(s => s.abilityId == id);
                if (slot == null)
                {
                    Debug.LogWarning(
                        $"[AbilityCooldownHUD] No UI slot for abilityId '{id}'. Add a slot with the same abilityId in the HUD inspector.");
                    continue;
                }

                if (slot.icon != null)
                {
                    if (ability.Data.icon != null)
                        slot.icon.sprite = ability.Data.icon;
                    else
                        Debug.LogWarning($"[AbilityCooldownHUD] No icon sprite on Ability Data '{ability.Data.name}'. Assign Icon in the SO.");
                }

                if (slot.keyHintText != null)
                    slot.keyHintText.text = KeyCodeToLabel(ability.Data.defaultKey, id);

                if (slot.cooldownOverlay != null)
                {
                    slot.cooldownOverlay.type = Image.Type.Filled;
                    if (slot.cooldownOverlay.fillMethod == Image.FillMethod.Horizontal)
                        slot.cooldownOverlay.fillOrigin = (int)Image.OriginHorizontal.Left;
                    slot.cooldownOverlay.fillAmount = 0f;
                    slot.cooldownOverlay.raycastTarget = false;
                }

                ability.OnCooldownUpdated += OnCooldownUpdated;

                if (logBinding)
                    Debug.Log($"[AbilityCooldownHUD] Bound slot '{id}' (cooldown {ability.Data.cooldown}s).");
            }
        }

        private void OnDestroy()
        {
            if (AbilityManager.Instance == null) return;
            foreach (AbilityBase ability in AbilityManager.Instance.GetAbilities())
            {
                if (ability != null)
                    ability.OnCooldownUpdated -= OnCooldownUpdated;
            }
        }

        private void OnCooldownUpdated(AbilityBase ability, float normalizedCooldown)
        {
            AbilitySlotUI slot = slots.Find(s => s.abilityId == ability.AbilityId);
            if (slot?.cooldownOverlay != null)
                slot.cooldownOverlay.fillAmount = normalizedCooldown;
        }

        private static string KeyCodeToLabel(KeyCode key, string abilityId)
        {
            if (key != KeyCode.None)
                return key.ToString();

            return abilityId switch
            {
                "jump" => "Space",
                "dash" => "Q",
                "projectile" => "F",
                _ => "?"
            };
        }
    }
}
