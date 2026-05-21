using UnityEngine;

namespace DungeonCrawler.Player
{
    public class PlayerDamageReceiver : MonoBehaviour
    {
        [SerializeField] private float invulnerabilityDuration = 0.6f;

        private float invulnerableUntil;

        public bool IsInvulnerable => Time.time < invulnerableUntil;

        public void ApplyInvulnerability()
        {
            invulnerableUntil = Time.time + invulnerabilityDuration;
        }
    }
}
