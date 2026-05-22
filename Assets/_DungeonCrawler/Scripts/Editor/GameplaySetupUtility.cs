#if UNITY_EDITOR
using DungeonCrawler.Combat;
using DungeonCrawler.Player;
using DungeonCrawler.Quests;
using UnityEditor;
using UnityEngine;

namespace DungeonCrawler.Editor
{
    public static class GameplaySetupUtility
    {
        private const string QuestAssetPath = "Assets/_DungeonCrawler/ScriptableObjects/Quests/TutorialEquipWeapon.asset";

        [MenuItem("Tools/Dungeon/Setup All Gameplay (Run Once)")]
        private static void SetupAll()
        {
            CombatAnimationEventSetup.SetupAllFromMenu();
            CreateTutorialEquipQuestAsset();
            SetupPlayerPrefab();
            SetupEnemyPrefab();
            SetupBossPrefab();
            AssetDatabase.SaveAssets();
            Debug.Log("[Dungeon] Gameplay setup complete. Enter Play mode to test.");
        }

        [MenuItem("Tools/Dungeon/Create Tutorial Equip Quest Asset")]
        private static void CreateTutorialEquipQuestAsset()
        {
            EnsureFolder("Assets/_DungeonCrawler/ScriptableObjects/Quests");

            QuestDataSO existing = AssetDatabase.LoadAssetAtPath<QuestDataSO>(QuestAssetPath);
            if (existing != null)
            {
                Debug.Log("[Dungeon] Tutorial equip quest asset already exists.");
                return;
            }

            QuestDataSO quest = ScriptableObject.CreateInstance<QuestDataSO>();
            quest.questID = EquipWeaponQuestBridge.QuestId;
            quest.questTitle = "Equip Your Weapon";
            quest.description = "Equip your weapon (Press E)";
            quest.objectiveType = ObjectiveType.EquipWeapon;
            quest.requiredAmount = 1;

            AssetDatabase.CreateAsset(quest, QuestAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Dungeon] Created {QuestAssetPath}");
        }

        private static void SetupPlayerPrefab()
        {
            const string path = "Assets/_DungeonCrawler/Scripts/Prefabs/Player.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return;

            EnsureComponent<PlayerGameplayBootstrap>(prefab);
            EnsureComponent<PlayerMovementLock>(prefab);
            EnsureComponent<WeaponEquipManager>(prefab);
            EnsureComponent<PlayerWeapon>(prefab);
            EnsureComponent<CombatAnimationEvents>(prefab);
            EnsureComponent<PlayerDeathHandler>(prefab);

            EditorUtility.SetDirty(prefab);
        }

        private static void SetupEnemyPrefab()
        {
            const string path = "Assets/_DungeonCrawler/Scripts/Prefabs/Enemy.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return;

            EnsureComponent<EntityGameplayBootstrap>(prefab);
            EnsureComponent<EntityDeathHandler>(prefab);
            EnsureComponent<CombatAnimationEvents>(prefab);

            EditorUtility.SetDirty(prefab);
        }

        private static void SetupBossPrefab()
        {
            const string path = "Assets/_DungeonCrawler/Scripts/Prefabs/Boss.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) return;

            EnsureComponent<EntityGameplayBootstrap>(prefab);
            EnsureComponent<EntityDeathHandler>(prefab);
            EnsureComponent<CombatAnimationEvents>(prefab);

            EditorUtility.SetDirty(prefab);
        }

        private static void EnsureComponent<T>(GameObject prefab) where T : Component
        {
            if (prefab.GetComponent<T>() == null)
                prefab.AddComponent<T>();
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
#endif
