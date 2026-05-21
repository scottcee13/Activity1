using UnityEngine;

public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }
    public EnemyState currentState;

    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Idle Settings")]
    public float idleDuration = 3f;

    private float idleTimer;
    private EnemyMovement movement;
    private EnemyAttack attack;
    private Animator animator;
    private DungeonCrawler.Combat.HealthComponent health;

    private void Start()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        animator = GetComponent<Animator>();
        health = GetComponent<DungeonCrawler.Combat.HealthComponent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        currentState = EnemyState.Idle;
    }

    private void Update()
    {
        if (health != null && health.IsDead) return;
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (animator != null)
                {
                    animator.SetBool("isPatrolling", false);
                    animator.SetBool("isChasing", false);
                }

                if (distance < detectionRange)
                {
                    idleTimer = 0;
                    currentState = EnemyState.Chase;
                }
                else
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleDuration)
                    {
                        idleTimer = 0;
                        currentState = EnemyState.Patrol;
                    }
                }
                break;

            case EnemyState.Patrol:
                if (animator != null)
                {
                    animator.SetBool("isPatrolling", true);
                    animator.SetBool("isChasing", false);
                }

                if (movement != null && movement.Patrol())
                    currentState = EnemyState.Idle;

                if (distance < detectionRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (animator != null)
                {
                    animator.SetBool("isPatrolling", false);
                    animator.SetBool("isChasing", true);
                }

                if (movement != null)
                    movement.MoveTo(player.position);

                if (distance <= attackRange)
                    currentState = EnemyState.Attack;
                else if (distance > detectionRange)
                    currentState = EnemyState.Idle;
                break;

            case EnemyState.Attack:
                if (animator != null)
                {
                    animator.SetBool("isPatrolling", false);
                    animator.SetBool("isChasing", false);
                }

                if (attack != null)
                    attack.Attack();

                if (distance > attackRange)
                    currentState = EnemyState.Chase;
                break;
        }
    }
}
