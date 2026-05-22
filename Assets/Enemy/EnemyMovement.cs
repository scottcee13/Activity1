using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public Transform[] patrolPoints;

    private int patrolIndex;
    private CharacterController controller;
    private NavMeshAgent agent;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            agent.speed = speed;
    }

    public void MoveTo(Vector3 target)
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.speed = speed;
            agent.SetDestination(target);

            Vector3 vel = agent.velocity;
            vel.y = 0f;
            if (vel.sqrMagnitude > 0.1f)
                transform.forward = vel.normalized;

            return;
        }

        if (controller == null) return;

        Vector3 direction = (target - transform.position);
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        direction.Normalize();
        controller.Move(direction * speed * Time.deltaTime);
        transform.forward = direction;
    }

    public bool Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return false;

        Transform target = patrolPoints[patrolIndex];
        if (target == null) return false;

        MoveTo(target.position);

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            return true;
        }

        return false;
    }
}
