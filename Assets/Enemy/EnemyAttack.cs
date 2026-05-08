using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Animator animator;
    public Transform attackPoint;
    public float attackRadius = 1.25f;
    public int damage = 10;
    public LayerMask playerLayer;

    float attackCooldown = 1.5f;
    float lastAttack;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (Time.time - lastAttack > attackCooldown)
        {
            animator.SetTrigger("Attack");
            DealDamage();
            lastAttack = Time.time;
        }
    }

    private void DealDamage()
    {
        if (attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}