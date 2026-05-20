using UnityEngine;

namespace DungeonCrawler.Inventory
{
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemDataSO item;
        [SerializeField] private bool destroyOnPickup = true;
        [SerializeField] private GameObject promptObject;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (promptObject != null) promptObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
                TryPickup();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (promptObject != null) promptObject.SetActive(false);
        }

        private void TryPickup()
        {
            if (item == null || InventoryManager.Instance == null) return;

            InventoryManager.Instance.AddItem(item);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(null);

            if (destroyOnPickup)
                Destroy(gameObject);
        }
    }
}
