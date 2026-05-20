using UnityEngine;

namespace DungeonCrawler.Core
{
    [RequireComponent(typeof(Collider))]
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private string roomId = "room_1";

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (CheckpointManager.Instance != null)
                CheckpointManager.Instance.SaveCheckpoint(other.transform, roomId);
        }
    }
}
