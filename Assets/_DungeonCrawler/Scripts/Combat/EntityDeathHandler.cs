using DungeonCrawler.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.Combat
{
    [RequireComponent(typeof(HealthComponent))]
    public class EntityDeathHandler : MonoBehaviour
    {
        [SerializeField] private float destroyDelay = 1.5f;
        [SerializeField] private bool destroyOnDeath = true;

        private HealthComponent health;
        private bool processed;

        private void Awake()
        {
            health = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            if (health != null)
                health.OnDeath -= HandleDeath;
        }

        public void ForceDeathCleanup()
        {
            HandleDeath();
        }

        private void HandleDeath()
        {
            if (processed) return;
            processed = true;

            StopMovementAndCombat();
            DisableAllColliders();
            HideRenderers();

            if (destroyOnDeath)
                Destroy(gameObject, destroyDelay);
        }

        private void StopMovementAndCombat()
        {
            EnemyFSM fsm = GetComponent<EnemyFSM>();
            if (fsm != null) fsm.enabled = false;

            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (attack != null) attack.enabled = false;

            EnemyMovement move = GetComponent<EnemyMovement>();
            if (move != null) move.enabled = false;

            BossController boss = GetComponent<BossController>();
            if (boss != null) boss.enabled = false;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                if (agent.enabled && agent.isOnNavMesh)
                    agent.isStopped = true;
                agent.velocity = Vector3.zero;
                agent.enabled = false;
            }

            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
                animator.enabled = false;
        }

        private void DisableAllColliders()
        {
            foreach (Collider col in GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        private void HideRenderers()
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }
    }
}
