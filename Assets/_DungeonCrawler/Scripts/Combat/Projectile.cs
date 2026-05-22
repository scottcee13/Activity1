using UnityEngine;

namespace DungeonCrawler.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 4f;
        [SerializeField] private float spawnIgnoreCollisionTime = 0.15f;
        [SerializeField] private float spawnForwardOffset = 0.75f;

        private int damage;
        private GameObject owner;
        private float spawnTime;
        private bool initialized;

        public void Initialize(int dmg, float speed, GameObject ownerObject, Vector3 direction)
        {
            damage = dmg;
            owner = ownerObject;
            spawnTime = Time.time;
            initialized = true;

            if (direction.sqrMagnitude < 0.001f)
                direction = transform.forward;

            direction.Normalize();

            transform.position += direction * spawnForwardOffset;
            transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = direction * speed;

            Collider col = GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;

            Physics.SyncTransforms();

            Destroy(gameObject, lifetime);
        }

        public void Initialize(int dmg, float speed, GameObject ownerObject)
        {
            Initialize(dmg, speed, ownerObject, transform.forward);
        }

        private void OnTriggerEnter(Collider other)
        {
            ProcessHit(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision != null)
                ProcessHit(collision.collider);
        }

        private void ProcessHit(Collider other)
        {
            if (!initialized || other == null) return;
            if (Time.time < spawnTime + spawnIgnoreCollisionTime) return;
            if (IsOwnerHierarchy(other.transform)) return;
            if (other.CompareTag("Player")) return;

            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health != null && !health.IsDead)
            {
                Vector3 dir = health.transform.position - transform.position;
                health.TakeDamage(damage, dir, 0f, false);
                Destroy(gameObject);
            }
        }

        private bool IsOwnerHierarchy(Transform other)
        {
            if (owner == null) return false;
            if (other.gameObject == owner) return true;
            return other.IsChildOf(owner.transform);
        }
    }
}
