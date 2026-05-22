using UnityEngine;

namespace DungeonCrawler.Player
{
    /// <summary>
    /// Authoritative flag for blocking locomotion during attacks, hit-stun, etc.
    /// </summary>
    public class PlayerMovementLock : MonoBehaviour
    {
        public static PlayerMovementLock Instance { get; private set; }

        private int lockCount;

        public bool IsLocked => lockCount > 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        public void LockMovement()
        {
            lockCount++;
        }

        public void UnlockMovement()
        {
            lockCount = Mathf.Max(0, lockCount - 1);
        }

        public void ForceUnlock()
        {
            lockCount = 0;
        }
    }
}
