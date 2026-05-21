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

    public void ApplyDamage(int dmg)
    {
        if (health <= 0 || dmg <= 0) return;
        Damage(dmg);
    }

    public void SyncFromHealthComponent(int currentHealth)
    {
        health = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Damage(int dmg)
    {
        health -= dmg;
        OnPlayerDamaged?.Invoke(health);
        if (health <= 0)
        {
            Die();
            return;
        }

        if (health <= maxHealth * 0.3f && !isInjured && animator != null)
            animator.SetLayerWeight(1, 1f);
    }
}
