using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.Boss
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossDataSO data;
        [SerializeField] private Transform player;
        [SerializeField] private float meleeRange = 3f;
        [SerializeField] private float knockbackForce = 6f;
        [SerializeField] private Transform rangedFirePoint;
        [SerializeField] private Combat.Projectile projectilePrefab;
        [SerializeField] private EnemySpawner addSpawner;
        [SerializeField] private Animator animator;
        [SerializeField] private bool waitForArenaActivation = true;

        private NavMeshAgent agent;
        private HealthComponent health;
        private float attackTimer;
        private int phase = 1;
        private bool isActivated;
        private bool agentReady;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<HealthComponent>();

            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }

            if (waitForArenaActivation)
            {
                isActivated = false;
                if (agent != null)
                    agent.enabled = false;
            }
            else
                ActivateBoss();
        }

        private void Start()
        {
            if (health != null)
                health.OnHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (health != null)
                health.OnHealthChanged -= OnHealthChanged;
        }

        public void ActivateBoss()
        {
            isActivated = true;

            if (agent == null) return;

            agent.enabled = true;

            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 8f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                agent.isStopped = false;
                agentReady = agent.isOnNavMesh;
            }
            else
            {
                agentReady = false;
                agent.enabled = false;
                Debug.LogWarning("[BossController] Boss is not on NavMesh. Bake NavMesh or move boss onto walkable floor.");
            }
        }

        private void Update()
        {
            if (!isActivated || health != null && health.IsDead) return;
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);

            if (agentReady && agent != null && agent.enabled && agent.isOnNavMesh)
                agent.SetDestination(player.position);

            attackTimer -= Time.deltaTime;
            if (attackTimer > 0f) return;

            if (dist <= meleeRange)
                MeleeAttack();
            else
                RangedAttack();

            attackTimer = data != null ? data.attackCooldown : 2f;
        }

        private void MeleeAttack()
        {
            if (animator != null) animator.SetTrigger("Attack");

            int dmg = data != null ? data.meleeDamage : 15;
            CombatDamage.Deal(player, dmg, transform, knockbackForce);
        }

        private void RangedAttack()
        {
            if (animator != null) animator.SetTrigger("RangedAttack");
            if (projectilePrefab == null || rangedFirePoint == null) return;

            Combat.Projectile proj = Instantiate(
                projectilePrefab,
                rangedFirePoint.position,
                rangedFirePoint.rotation
            );
            proj.Initialize(data != null ? data.rangedDamage : 10, 20f, gameObject);
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
