using DungeonCrawler.Combat;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 12;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float attackReach = 2.5f;
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private string attackTrigger = "Attack";

    private Animator animator;
    private float lastAttackTime;
    private Transform player;
    private bool attackInProgress;
    private bool hitAppliedThisSwing;
    private HealthComponent selfHealth;

    public bool IsAttacking => attackInProgress;
    public bool CanStartAttack => !attackInProgress && Time.time >= lastAttackTime + attackCooldown;

    private void Start()
    {
        animator = GetComponent<Animator>();
        selfHealth = GetComponent<HealthComponent>();
        FindPlayer();
    }

    private void Update()
    {
        if (selfHealth != null && selfHealth.IsDead)
            enabled = false;
    }

    public void ConfigureDamage(int damage, float cooldown)
    {
        attackDamage = damage;
        attackCooldown = cooldown;
    }

    public void ConfigureReach(float reach)
    {
        attackReach = reach;
    }

    public bool TryBeginAttack()
    {
        if (!CanStartAttack) return false;
        if (selfHealth != null && selfHealth.IsDead) return false;

        FindPlayer();
        if (player == null) return false;

        FacePlayer();

        attackInProgress = true;
        hitAppliedThisSwing = false;
        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.ResetTrigger(attackTrigger);
            animator.SetTrigger(attackTrigger);
        }

        if (attackSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(attackSfx);

        return true;
    }

    public void OnAttackHit()
    {
        if (!attackInProgress || hitAppliedThisSwing) return;
        if (selfHealth != null && selfHealth.IsDead) return;

        hitAppliedThisSwing = true;
        ApplyDamageToPlayer();
    }

    public void OnAttackEnd()
    {
        attackInProgress = false;
        hitAppliedThisSwing = false;

        if (animator != null)
            animator.ResetTrigger(attackTrigger);
    }

    private void ApplyDamageToPlayer()
    {
        FindPlayer();
        if (player == null) return;

        HealthComponent playerHealth = player.GetComponent<HealthComponent>();
        if (playerHealth != null && playerHealth.IsDead) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackReach) return;

        CombatDamage.Deal(player, attackDamage, transform, knockbackForce);

        if (hitSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hitSfx);

        Animator playerAnimator = player.GetComponentInChildren<Animator>();
        if (playerAnimator != null)
            playerAnimator.SetTrigger("hurt");
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void FindPlayer()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
}
