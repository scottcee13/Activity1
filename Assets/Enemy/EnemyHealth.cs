using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public int injuredThreshold = 30;

    Animator animator;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
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
        currentHP -= damage;
    }

}