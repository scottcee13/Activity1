using UnityEngine;

namespace DungeonCrawler.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 4f;

        private int damage;
        private GameObject owner;

        public void Initialize(int dmg, float speed, GameObject ownerObject, Vector3 direction)
        {
            damage = dmg;
            owner = ownerObject;

            if (direction.sqrMagnitude < 0.001f)
                direction = transform.forward;

            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.linearVelocity = direction * speed;

            Destroy(gameObject, lifetime);
        }

        // Legacy overload
        public void Initialize(int dmg, float speed, GameObject ownerObject)
        {
            Initialize(dmg, speed, ownerObject, transform.forward);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (owner != null && collision.gameObject == owner) return;
            if (collision.gameObject.CompareTag("Player")) return;

            HealthComponent health = collision.collider.GetComponentInParent<HealthComponent>();
            if (health != null)
                health.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
