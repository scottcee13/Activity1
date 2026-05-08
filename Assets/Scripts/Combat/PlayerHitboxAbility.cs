using UnityEngine;

public class PlayerHitboxAbility : MonoBehaviour
{
    [Header("Attack Ability")]
    [SerializeField] private KeyCode abilityKey = KeyCode.Mouse0;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float range = 1.6f;
    [SerializeField] private int damage = 25;
    [SerializeField] private float cooldown = 0.6f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private AudioClip hitSfx;

    private float lastCastTime = -999f;

    private void Update()
    {
        if (Input.GetKeyDown(abilityKey))
        {
            TryCast();
        }
    }

    public void TryCast()
    {
        if (Time.time < lastCastTime + cooldown) return;
        if (attackPoint == null) return;

        lastCastTime = Time.time;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, range, enemyLayer);
        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && enemyHealth.IsAlive())
            {
                enemyHealth.TakeDamage(damage);
                if (AudioManager.Instance != null && hitSfx != null)
                {
                    AudioManager.Instance.PlaySFX(hitSfx);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPoint.position, range);
    }
}
