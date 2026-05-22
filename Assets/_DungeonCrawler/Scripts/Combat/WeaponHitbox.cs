using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Combat
{
    public class WeaponHitbox : MonoBehaviour
    {
        [SerializeField] private Collider hitCollider;
        [SerializeField] private int damage = 15;
        [SerializeField] private float knockbackForce = 12f;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private Transform knockbackSource;
        [SerializeField] private AudioClip hitSfx;
        [SerializeField] private GameObject hitVfxPrefab;
        [SerializeField] private bool applyDamageOnHit = true;

        private readonly HashSet<HealthComponent> hitThisSwing = new HashSet<HealthComponent>();
        private bool swingActive;

        private void Awake()
        {
            if (hitCollider == null)
                hitCollider = GetComponent<Collider>();

            if (hitCollider != null)
            {
                hitCollider.isTrigger = true;
                hitCollider.enabled = false;
            }

            if (targetLayers.value == 0 || targetLayers.value == ~0)
            {
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer >= 0)
                    targetLayers = 1 << enemyLayer;
            }
        }

        public void SetDamage(int value) => damage = Mathf.Max(1, value);
        public void SetKnockback(float value) => knockbackForce = Mathf.Max(0f, value);
        public void SetHitSfx(AudioClip clip) => hitSfx = clip;
        public void SetApplyDamage(bool value) => applyDamageOnHit = value;

        public void BeginSwing()
        {
            hitThisSwing.Clear();
            swingActive = true;
            if (hitCollider != null)
                hitCollider.enabled = true;
        }

        public void EndSwing()
        {
            swingActive = false;
            if (hitCollider != null)
                hitCollider.enabled = false;
            hitThisSwing.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!swingActive || hitCollider == null || !hitCollider.enabled) return;
            if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health == null || health.IsDead || hitThisSwing.Contains(health)) return;

            hitThisSwing.Add(health);

            if (applyDamageOnHit)
            {
                Transform source = knockbackSource != null ? knockbackSource : transform;
                Vector3 dir = health.transform.position - source.position;
                health.TakeDamage(damage, dir, knockbackForce, true);
            }

            if (hitSfx != null && AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(hitSfx);

            if (hitVfxPrefab != null)
                Instantiate(hitVfxPrefab, other.ClosestPoint(transform.position), Quaternion.identity);
        }
    }
}
