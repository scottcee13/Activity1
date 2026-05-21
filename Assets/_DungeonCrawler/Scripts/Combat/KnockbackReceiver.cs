using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Knockback: instant shove + short follow-through. Uses LateUpdate so movement scripts don't cancel it.
    /// </summary>
    public class KnockbackReceiver : MonoBehaviour
    {
        [SerializeField] private float knockbackDecay = 8f;
        [SerializeField] private float shoveMultiplier = 0.2f;
        [SerializeField] private float stunDuration = 0.25f;

        private CharacterController characterController;
        private NavMeshAgent navAgent;
        private Vector3 knockbackVelocity;
        private Coroutine stunRoutine;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            navAgent = GetComponent<NavMeshAgent>();
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            if (force <= 0f) return;

            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f)
                direction = -transform.forward;

            direction.Normalize();
            knockbackVelocity = direction * force;

            Vector3 instantShove = direction * (force * shoveMultiplier);

            if (characterController != null && characterController.enabled)
                characterController.Move(instantShove);
            else if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
            {
                navAgent.Move(instantShove);
                if (navAgent.isOnNavMesh)
                    navAgent.velocity = direction * force * 0.35f;
            }
            else
                transform.position += instantShove;

            EnemyFSM fsm = GetComponent<EnemyFSM>();
            if (fsm != null)
            {
                if (stunRoutine != null)
                    StopCoroutine(stunRoutine);
                stunRoutine = StartCoroutine(StunEnemyAi(fsm));
            }
        }

        private IEnumerator StunEnemyAi(EnemyFSM fsm)
        {
            fsm.enabled = false;
            yield return new WaitForSeconds(stunDuration);

            if (fsm != null)
            {
                HealthComponent health = GetComponent<HealthComponent>();
                if (health == null || !health.IsDead)
                    fsm.enabled = true;
            }

            stunRoutine = null;
        }

        private void LateUpdate()
        {
            if (knockbackVelocity.sqrMagnitude < 0.01f) return;

            Vector3 delta = knockbackVelocity * Time.deltaTime;

            if (characterController != null && characterController.enabled)
                characterController.Move(delta);
            else if (navAgent != null && navAgent.enabled && navAgent.isOnNavMesh)
                navAgent.Move(delta);
            else
                transform.position += delta;

            knockbackVelocity = Vector3.Lerp(
                knockbackVelocity,
                Vector3.zero,
                knockbackDecay * Time.deltaTime
            );
        }
    }
}
