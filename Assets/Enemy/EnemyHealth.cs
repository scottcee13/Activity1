using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public int injuredThreshold = 30;

    Animator animator;
    private bool isDead;
    public static Action<EnemyHealth> OnEnemyKilled;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        isDead = false;
    }

    void Update()
    {
        if (currentHP <= injuredThreshold)
        {
            animator.SetLayerWeight(1, 1);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.P) && currentHP > 0)
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject, 0.25f);
    }

    public bool IsAlive()
    {
        return !isDead;
    }
}