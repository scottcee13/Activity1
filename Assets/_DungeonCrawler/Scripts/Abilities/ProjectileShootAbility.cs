using DungeonCrawler.Combat;
using DungeonCrawler.Player;
using UnityEngine;

namespace DungeonCrawler.Abilities
{
    /// <summary>
    /// Spawns projectile at FirePoint, flies toward camera aim (PlayerAimProvider).
    /// </summary>
    public class ProjectileShootAbility : AbilityBase
    {
        [SerializeField] private Transform firePoint;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int damage = 15;
        [SerializeField] private float projectileSpeed = 25f;
        [SerializeField] private bool autoCreateFirePoint = true;

        private void Awake()
        {
            if (firePoint == null)
                firePoint = transform.Find("FirePoint");

            if (firePoint == null && autoCreateFirePoint)
            {
                GameObject fp = new GameObject("FirePoint");
                fp.transform.SetParent(transform);
                fp.transform.localPosition = new Vector3(0f, 1.4f, 0.6f);
                fp.transform.localRotation = Quaternion.identity;
                firePoint = fp.transform;
                Debug.Log("[ProjectileShootAbility] Created FirePoint — adjust in Inspector (chest height, in front of model).");
            }
        }

        protected override bool CanExecute()
        {
            return firePoint != null && projectilePrefab != null;
        }

        protected override void Execute()
        {
            Vector3 direction = GetAimDirection();
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Spawn at fire point, facing aim direction (not world up).
            Projectile proj = Instantiate(
                projectilePrefab,
                firePoint.position,
                rotation
            );
            proj.Initialize(damage, projectileSpeed, gameObject, direction);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(null);
        }

        private Vector3 GetAimDirection()
        {
            if (PlayerAimProvider.Instance != null)
                return PlayerAimProvider.Instance.AimDirection3D;

            if (Camera.main != null)
                return Camera.main.transform.forward;

            return transform.forward;
        }
    }
}
