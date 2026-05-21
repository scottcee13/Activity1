using DungeonCrawler.Combat;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 12;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float damageDelay = 0.45f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float attackReach = 2.5f;
    [SerializeField] private AudioClip attackSfx;

    private Animator animator;
    private float lastAttack;
    private Transform player;
    private bool damagePending;

    private void Start()
    {
        animator = GetComponent<Animator>();
        FindPlayer();
    }

    private void Update()
    {
        if (!damagePending) return;

        damageDelay -= Time.deltaTime;
        if (damageDelay <= 0f)
        {
            damagePending = false;
            ApplyDamageToPlayer();
        }
    }

    public void Attack()
    {
        if (Time.time - lastAttack < attackCooldown) return;

        lastAttack = Time.time;
        damagePending = true;
        damageDelay = 0.45f;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (attackSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(attackSfx);
    }

    public void OnAttackHit()
    {
        damagePending = false;
        ApplyDamageToPlayer();
    }

    private void ApplyDamageToPlayer()
    {
        FindPlayer();
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackReach) return;

        CombatDamage.Deal(player, attackDamage, transform, knockbackForce);
    }

    private void FindPlayer()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
}
