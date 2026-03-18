using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    public Transform[] patrolPoints;

    int patrolIndex = 0;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        direction.y = 0;

        controller.Move(direction * speed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
        }
    }

    public bool Patrol() // Changed from void to bool
    {
        if (patrolPoints.Length == 0) return false;

        Transform target = patrolPoints[patrolIndex];
        MoveTo(target.position);

        // Check if we are close enough to the waypoint
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            // Prepare the next index for NEXT time we patrol
            patrolIndex++;
            if (patrolIndex >= patrolPoints.Length)
                patrolIndex = 0;

            return true; // We reached the point!
        }

        return false; // We are still walking
    }
}