using DungeonCrawler.UI;
using UnityEngine;

namespace DungeonCrawler.World
{
    [RequireComponent(typeof(Collider))]
    public class ControlPromptTrigger : MonoBehaviour
    {
        [SerializeField] private string promptMessage = "WASD — Move | E — Interact | LMB — Attack";
        [SerializeField] private float displayDuration = 5f;
        [SerializeField] private bool requirePlayerTag = true;

        private bool shown;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (requirePlayerTag && !other.CompareTag("Player"))
                return;

            if (shown) return;
            shown = true;

            if (ControlPromptUI.Instance == null)
            {
                Debug.LogWarning(
                    $"[ControlPromptTrigger] No ControlPromptUI in scene! Message was: {promptMessage}\n" +
                    "Create Canvas → ControlPromptPanel with ControlPromptUI script.");
                return;
            }

            ControlPromptUI.Instance.Show(promptMessage, displayDuration);
        }
    }
}
