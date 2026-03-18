using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Animator animator;

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
            lastAttack = Time.time;
        }
    }
}