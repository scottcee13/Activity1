using UnityEngine;

namespace DungeonCrawler.Abilities
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Dungeon/Ability Data")]
    public class AbilityDataSO : ScriptableObject
    {
        public string abilityId;
        public string displayName;
        [TextArea] public string description;
        public float cooldown = 2f;
        public Sprite icon;
        public KeyCode defaultKey = KeyCode.None;
    }
}
