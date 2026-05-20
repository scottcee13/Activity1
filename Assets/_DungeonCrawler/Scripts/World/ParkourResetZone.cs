using DungeonCrawler.Player;
using UnityEngine;

namespace DungeonCrawler.World
{
    /// <summary>
    /// Room 3: teleports player to respawn when they fall or fail parkour.
    /// </summary>
    public class ParkourResetZone : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private bool resetVelocity = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Transform target = respawnPoint != null ? respawnPoint : transform;
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            other.transform.position = target.position;
            other.transform.rotation = target.rotation;

            if (cc != null) cc.enabled = true;

            if (resetVelocity)
            {
                PlayerMotor motor = other.GetComponent<PlayerMotor>();
                motor?.ResetVerticalVelocity();
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(null);

            Debug.Log("[Parkour] Reset to checkpoint");
        }
    }
}
