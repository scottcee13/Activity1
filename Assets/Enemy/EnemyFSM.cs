using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }
    public EnemyState currentState;

    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Idle Settings")]
    public float idleDuration = 3f; // How long to wait
    private float idleTimer;        // The actual clock

    EnemyMovement movement;
    EnemyAttack attack;
    Animator animator;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        animator = GetComponent<Animator>();

        currentState = EnemyState.Idle;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.SetBool("isPatrolling", false);
                animator.SetBool("isChasing", false);

                // 1. Always check if we should Chase first (Interrupt the wait)
                if (distance < detectionRange)
                {
                    idleTimer = 0; // Reset timer for next time
                    currentState = EnemyState.Chase;
                }
                else
                {
                    // 2. Otherwise, count up the timer
                    idleTimer += Time.deltaTime;

                    if (idleTimer >= idleDuration)
                    {
                        idleTimer = 0; // Reset timer
                        currentState = EnemyState.Patrol;
                    }
                }
                break;

            case EnemyState.Patrol:
                animator.SetBool("isPatrolling", true);
                animator.SetBool("isChasing", false);

                // Capture the "Reached" signal from the movement script
                bool reachedWaypoint = movement.Patrol();

                if (reachedWaypoint)
                {
                    currentState = EnemyState.Idle; // This starts the idle timer automatically!
                }

                if (distance < detectionRange)
                {
                    currentState = EnemyState.Chase;
                }
                break;

            case EnemyState.Chase:
                animator.SetBool("isPatrolling", false);
                animator.SetBool("isChasing", true);

                movement.MoveTo(player.position);

                if (distance <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }

                if (distance > detectionRange)
                {
                    // When the player escapes, we go to Idle to "rest"
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attack:
                animator.SetBool("isPatrolling", false);
                animator.SetBool("isChasing", false);

                attack.Attack();

                if (distance > attackRange)
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }
}