using UnityEngine;

namespace DungeonCrawler.Combat
{
    /// <summary>
    /// Ensures death + combat event components exist on enemies and bosses.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    public class EntityGameplayBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            if (GetComponent<HealthComponent>() == null)
                gameObject.AddComponent<HealthComponent>();

            if (GetComponent<EntityDeathHandler>() == null)
                gameObject.AddComponent<EntityDeathHandler>();

            if (GetComponent<CombatAnimationEvents>() == null)
                gameObject.AddComponent<CombatAnimationEvents>();

            if (GetComponent<EnemyHurtbox>() == null)
                gameObject.AddComponent<EnemyHurtbox>();
        }
    }
}
