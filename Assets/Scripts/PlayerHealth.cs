using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int health = 100;
    public static Action<int> OnPlayerDamaged;
    public static Action OnPlayerDeath;

    // Update is called once per frame
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
    }
}
