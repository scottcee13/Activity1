using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Trigger collider on weapon; enable only during attack window.
    /// </summary>
    public class WeaponHitbox : MonoBehaviour
    {
        [SerializeField] private Collider hitCollider;
        [SerializeField] private int damage = 15;
        [SerializeField] private float knockbackForce = 12f;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private Transform knockbackSource;

        private readonly HashSet<HealthComponent> hitThisSwing = new HashSet<HealthComponent>();

        private void Awake()
        {
            if (hitCollider == null)
                hitCollider = GetComponent<Collider>();

            if (hitCollider != null)
            {
                hitCollider.isTrigger = true;
                hitCollider.enabled = false;
            }
        }

        public void SetDamage(int value) => damage = Mathf.Max(1, value);

        public void BeginSwing()
        {
            hitThisSwing.Clear();
            if (hitCollider != null)
                hitCollider.enabled = true;
        }

        public void EndSwing()
        {
            if (hitCollider != null)
                hitCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hitCollider == null || !hitCollider.enabled) return;
            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health == null || health.IsDead || hitThisSwing.Contains(health)) return;

            hitThisSwing.Add(health);

            Transform source = knockbackSource != null ? knockbackSource : transform;
            Vector3 dir = health.transform.position - source.position;
            health.TakeDamage(damage, dir, knockbackForce);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(null);
        }
    }
}
