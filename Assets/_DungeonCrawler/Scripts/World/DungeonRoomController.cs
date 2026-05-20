using DungeonCrawler.Core;
using UnityEngine;

namespace DungeonCrawler.World
{
    /// <summary>
    /// Marks a dungeon zone and reacts when the player enters.
    /// </summary>
    public class DungeonRoomController : MonoBehaviour
    {
        [SerializeField] private DungeonRoomType roomType;
        [SerializeField] private string roomId;
        [SerializeField] private bool triggerVictoryOnEnter;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            GameEvents.RaiseRoomEntered(roomId);

            switch (roomType)
            {
                case DungeonRoomType.Reward:
                    if (triggerVictoryOnEnter && GameManager.Instance != null)
                        GameManager.Instance.TriggerVictory();
                    break;
            }

            Debug.Log($"[Room] Entered {roomType} ({roomId})");
        }
    }
}
