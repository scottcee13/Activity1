using DungeonCrawler.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.Boss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossDataSO data;
        [SerializeField] private Transform player;
        [SerializeField] private float detectionRange = 30f;
        [SerializeField] private float meleeRange = 6f;
        [SerializeField] private float rangedMinDistance = 8f;
        [SerializeField] private Transform rangedFirePoint;
        [SerializeField] private Combat.Projectile projectilePrefab;
        [SerializeField] private EnemySpawner addSpawner;
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyAttack meleeAttack;
        [SerializeField] private bool waitForArenaActivation;
        [SerializeField] private bool autoActivateWhenPlayerNear = true;
        [SerializeField] private string meleeTrigger = "Attack";
        [SerializeField] private string rangedTrigger = "RangedAttack";
        [SerializeField] private string attackStateName = "Attack";
        [SerializeField] private float locomotionSpeedThreshold = 0.15f;

        private NavMeshAgent agent;
        private HealthComponent health;
        private CharacterController characterController;
        private float attackCooldownTimer;
        private int phase = 1;
        private bool isActivated;
        private bool agentReady;
        private bool attackInProgress;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<HealthComponent>();
            characterController = GetComponent<CharacterController>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            if (meleeAttack == null)
                meleeAttack = GetComponent<EnemyAttack>();

            EnemyFSM fsm = GetComponent<EnemyFSM>();
            if (fsm != null) fsm.enabled = false;

            EnemyMovement move = GetComponent<EnemyMovement>();
            if (move != null) move.enabled = false;

            if (characterController != null)
                characterController.enabled = false;

            EnemyHurtbox hurtbox = GetComponent<EnemyHurtbox>();
            if (hurtbox == null)
                hurtbox = gameObject.AddComponent<EnemyHurtbox>();
            hurtbox.BuildHurtbox();

            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z, 1f);
            meleeRange = Mathf.Max(meleeRange, 4f * scale);

            if (agent != null)
            {
                agent.stoppingDistance = Mathf.Max(1.5f, meleeRange * 0.75f);
                agent.autoBraking = true;
                agent.acceleration = 12f;
                agent.speed = Mathf.Max(agent.speed, 4f);
            }

            if (meleeAttack != null)
            {
                meleeAttack.ConfigureReach(meleeRange + 2f);
                if (data != null)
                    meleeAttack.ConfigureDamage(data.meleeDamage, data.attackCooldown);
            }

            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (!waitForArenaActivation)
                ActivateBoss();
            else if (agent != null)
                agent.enabled = false;
        }

        private void Start()
        {
            if (health != null)
                health.OnHealthChanged += OnHealthChanged;

            if (!waitForArenaActivation)
                ActivateBoss();
        }

        private void OnDestroy()
        {
            if (health != null)
                health.OnHealthChanged -= OnHealthChanged;
        }

        public void ActivateBoss()
        {
            if (isActivated) return;
            isActivated = true;

            if (agent == null) return;

            agent.enabled = true;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 15f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                agent.isStopped = false;
                agentReady = agent.isOnNavMesh;
            }
            else
            {
                agentReady = false;
                Debug.LogWarning("[BossController] Boss not on NavMesh — bake NavMesh under the boss.");
            }
        }

        private void Update()
        {
            if (health != null && health.IsDead) return;
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);

            if (!isActivated && autoActivateWhenPlayerNear && dist <= detectionRange)
                ActivateBoss();

            if (!isActivated) return;

            if (dist > detectionRange)
            {
                UpdateLocomotionAnimator(false);
                return;
            }

            FacePlayer();

            if (attackInProgress)
            {
                PollAttackEnd();
                StopAgent();
                UpdateLocomotionAnimator(false);
                return;
            }

            bool inMeleeRange = dist <= meleeRange;
            UpdateChase(dist, inMeleeRange);
            UpdateLocomotionAnimator(!inMeleeRange && IsAgentMoving());

            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
                return;
            }

            if (inMeleeRange)
            {
                if (TryMeleeAttack())
                    attackCooldownTimer = data != null ? data.attackCooldown : 2f;
            }
            else if (dist >= rangedMinDistance)
            {
                TryRangedAttack();
                attackCooldownTimer = data != null ? data.attackCooldown : 2f;
            }
        }

        private void UpdateChase(float dist, bool inMelee)
        {
            if (!agentReady || agent == null || !agent.enabled || !agent.isOnNavMesh) return;

            agent.isStopped = inMelee;
            agent.stoppingDistance = Mathf.Max(1.5f, meleeRange * 0.75f);

            if (!inMelee)
                agent.SetDestination(player.position);
        }

        private bool IsAgentMoving()
        {
            if (agent == null || !agent.enabled || agent.isStopped) return false;
            return agent.velocity.sqrMagnitude > locomotionSpeedThreshold * locomotionSpeedThreshold;
        }

        private void UpdateLocomotionAnimator(bool chasing)
        {
            if (animator == null) return;

            animator.SetBool("isChasing", chasing);
            animator.SetBool("isPatrolling", false);
        }

        private void StopAgent()
        {
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.isStopped = true;
        }

        private bool TryMeleeAttack()
        {
            StopAgent();
            FacePlayer(true);

            if (meleeAttack != null)
            {
                if (meleeAttack.TryBeginAttack())
                {
                    attackInProgress = true;
                    return true;
                }
                return false;
            }

            attackInProgress = true;
            if (animator != null)
            {
                animator.ResetTrigger(meleeTrigger);
                animator.SetTrigger(meleeTrigger);
            }

            return true;
        }

        private void TryRangedAttack()
        {
            if (animator != null)
            {
                animator.ResetTrigger(rangedTrigger);
                animator.SetTrigger(rangedTrigger);
            }

            if (projectilePrefab == null || rangedFirePoint == null || player == null) return;

            Vector3 dir = player.position + Vector3.up * 1.2f - rangedFirePoint.position;
            if (dir.sqrMagnitude < 0.01f) return;

            dir.Normalize();
            Combat.Projectile proj = Instantiate(projectilePrefab, rangedFirePoint.position, Quaternion.LookRotation(dir));
            proj.Initialize(data != null ? data.rangedDamage : 10, 20f, gameObject, dir);
        }

        public void OnAttackHit()
        {
            if (meleeAttack != null)
                meleeAttack.OnAttackHit();
            else
                CombatDamage.Deal(player, data != null ? data.meleeDamage : 15, transform, 6f);
        }

        public void OnAttackEnd()
        {
            attackInProgress = false;
            if (meleeAttack != null)
                meleeAttack.OnAttackEnd();

            if (animator != null)
                animator.ResetTrigger(meleeTrigger);
        }

        private void PollAttackEnd()
        {
            if (animator == null) return;

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (animator.IsInTransition(0)) return;

            if (state.IsName(attackStateName) && state.normalizedTime >= 0.92f)
                OnAttackEnd();
            else if (!state.IsName(attackStateName))
                OnAttackEnd();
        }

        private void FacePlayer(bool instant = false)
        {
            if (player == null) return;

            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) return;

            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = instant
                ? look
                : Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }

        private void OnHealthChanged(int current, int max)
        {
            if (data == null) return;
            float pct = (float)current / max;
            if (phase == 1 && pct <= data.phase2HealthPercent)
            {
                phase = 2;
                addSpawner?.SpawnWave(0);
                if (animator != null) animator.SetBool("Enraged", true);
            }
        }
    }
}
