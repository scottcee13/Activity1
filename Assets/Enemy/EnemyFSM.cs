using DungeonCrawler.Boss;
using DungeonCrawler.Combat;
using UnityEngine;
using UnityEngine.AI;

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
    private HealthComponent health;
    private NavMeshAgent agent;

    private void Start()
    {
        movement = GetComponent<EnemyMovement>();
        attack = GetComponent<EnemyAttack>();
        animator = GetComponent<Animator>();
        health = GetComponent<HealthComponent>();
        agent = GetComponent<NavMeshAgent>();

        if (GetComponent<BossController>() != null)
        {
            enabled = false;
            return;
        }

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
        if (!enabled) return;
        if (health != null && health.IsDead)
        {
            enabled = false;
            return;
        }

        if (player == null) return;

        if (attack != null && attack.IsAttacking)
        {
            StopAgent();
            SetAnimatorLocomotion(false, false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                StopAgent();
                SetAnimatorLocomotion(false, false);

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
                SetAnimatorLocomotion(true, false);

                if (movement != null && movement.Patrol())
                    currentState = EnemyState.Idle;

                if (distance < detectionRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                SetAnimatorLocomotion(false, true);

                if (movement != null)
                    movement.MoveTo(player.position);

                if (distance <= attackRange)
                    currentState = EnemyState.Attack;
                else if (distance > detectionRange * 1.2f)
                    currentState = EnemyState.Idle;
                break;

            case EnemyState.Attack:
                StopAgent();
                SetAnimatorLocomotion(false, false);
                FacePlayer();

                if (attack != null && attack.TryBeginAttack())
                {
                    // Wait in Attack until OnAttackEnd clears attackInProgress
                }
                else if (attack == null || !attack.IsAttacking)
                {
                    if (distance > attackRange)
                        currentState = EnemyState.Chase;
                }
                break;
        }
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            8f * Time.deltaTime
        );
    }

    private void StopAgent()
    {
        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    private void SetAnimatorLocomotion(bool patrolling, bool chasing)
    {
        if (animator == null) return;
        animator.SetBool("isPatrolling", patrolling);
        animator.SetBool("isChasing", chasing);
    }
}
