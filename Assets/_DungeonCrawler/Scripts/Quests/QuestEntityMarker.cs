using DungeonCrawler.Combat;
using UnityEngine;

namespace DungeonCrawler.Quests
{
    /// <summary>
    /// Assign a unique entity id on this enemy/boss instance (e.g. first_enemy, dungeon_boss).
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class QuestEntityMarker : MonoBehaviour
    {
        [SerializeField] private string entityId = "first_enemy";

        private void Awake()
        {
            HealthComponent health = GetComponent<HealthComponent>();
            if (health != null && !string.IsNullOrEmpty(entityId))
                health.ConfigureEntityId(entityId);
        }
    }
}
