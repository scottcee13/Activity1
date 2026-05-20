using DungeonCrawler.Combat;
using DungeonCrawler.Core;
using UnityEngine;
using UnityEngine.AI;

namespace DungeonCrawler.Boss
{
    /// <summary>
    /// Boss AI: chase player, alternate melee/ranged by phase, spawn adds via EnemySpawner.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private BossDataSO data;
        [SerializeField] private Transform player;
        [SerializeField] private float meleeRange = 3f;
        [SerializeField] private Transform rangedFirePoint;
        [SerializeField] private Combat.Projectile projectilePrefab;
        [SerializeField] private EnemySpawner addSpawner;
        [SerializeField] private Animator animator;

        private NavMeshAgent agent;
        private HealthComponent health;
        private float attackTimer;
        private int phase = 1;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<HealthComponent>();

            if (health != null && data != null)
            {
                // Max health configured on HealthComponent in inspector; sync id
            }

            if (player == null)
            {
                GameObject p = GameObject.FindGameObjectWithTag("Player");
                if (p != null) player = p.transform;
            }
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

        private void Update()
        {
            if (health != null && health.IsDead) return;
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.position);
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

            HealthComponent playerHealth = player.GetComponent<HealthComponent>();
            if (playerHealth == null) playerHealth = player.GetComponentInParent<HealthComponent>();

            int dmg = data != null ? data.meleeDamage : 15;
            playerHealth?.TakeDamage(dmg);
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
