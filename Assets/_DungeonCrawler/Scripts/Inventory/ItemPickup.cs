using DungeonCrawler.UI;
using UnityEngine;

namespace DungeonCrawler.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemDataSO item;
        [SerializeField] private bool destroyOnPickup = true;
        [SerializeField] private GameObject promptObject;
        [SerializeField] private string promptMessage = "Press E to Pick Up";
        [SerializeField] private AudioClip pickupSfx;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (promptObject != null)
                promptObject.SetActive(true);
            else if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Show(promptMessage);

            if (Input.GetKeyDown(KeyCode.E))
                TryPickup();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (promptObject != null)
                promptObject.SetActive(false);
            else if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.Hide();
        }

        private void TryPickup()
        {
            if (item == null || InventoryManager.Instance == null) return;

            InventoryManager.Instance.AddItem(item);

            if (AudioManager.Instance != null && pickupSfx != null)
                AudioManager.Instance.PlaySFX(pickupSfx);

            if (InteractPromptUI.Instance != null)
                InteractPromptUI.Instance.ForceHide();

            if (destroyOnPickup)
                Destroy(gameObject);
        }
    }
}
