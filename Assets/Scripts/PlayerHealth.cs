using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    public int health;
    public static Action<int> OnPlayerDamaged;
    public static Action OnPlayerDeath;

    private bool isInjured = false;
    // Update is called once per frame

    private void Start()
    {
        health = maxHealth;
        animator.SetLayerWeight(1, 0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && health > 0)
        {
            Damage(10);
        }
    }

    void Die()
    {
        OnPlayerDeath?.Invoke();
    }

    void Damage(int dmg)
    {
        health -= dmg;
        Debug.Log(health);
        OnPlayerDamaged?.Invoke(health);
        if (health <= 0)
        {
            Die();
        }
        if (health <= maxHealth * 0.3f && !isInjured)
        {
            //isInjured = true;
            //animator.SetBool("Injured",true);
            animator.SetLayerWeight(1, 1f);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;
        Damage(damageAmount);
    }
}
