#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DungeonCrawler.Editor
{
    /// <summary>
    /// FBX clips are read-only in the Animation window. This tool copies them to writable .anim files
    /// with combat events baked in, then optionally assigns them to animator states.
    /// </summary>
    public static class CombatAnimationEventSetup
    {
        private const string OutputFolder = "Assets/_DungeonCrawler/Animations/CombatClips";
        private const float HitNormalizedTime = 0.45f;
        private const float EndNormalizedTime = 0.95f;

        private static readonly SetupEntry[] DefaultEntries =
        {
            new SetupEntry(
                "Assets/Animations/Kachujin G Rosales@Standing Melee Attack Downward.fbx",
                "Player_PrimaryAttack_Combat.anim",
                "Assets/Animations/Character.controller",
                "PrimaryAttack"),
            new SetupEntry(
                "Assets/Animations/Kachujin G Rosales@Standing Melee Attack Backhand.fbx",
                "Player_SecondaryAttack_Combat.anim",
                "Assets/Animations/Character.controller",
                "SecondaryAttack"),
            new SetupEntry(
                "Assets/Enemy/Paladin WProp J Nordstrom@Sword And Shield Slash.fbx",
                "Enemy_Attack_Combat.anim",
                "Assets/Enemy/Enemy.controller",
                "Attack")
        };

        public static void SetupAllFromMenu() => SetupAll();

        [MenuItem("Tools/Dungeon/Setup Combat Animation Events (Copy Clips)")]
        private static void SetupAll()
        {
            EnsureFolder(OutputFolder);
            int created = 0;

            foreach (SetupEntry entry in DefaultEntries)
            {
                if (ProcessEntry(entry))
                    created++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Dungeon] Created/updated {created} writable combat clip(s) in {OutputFolder}. " +
                      "Animator states were updated where controllers were found.");
        }

        [MenuItem("Tools/Dungeon/Setup Combat Animation Events (Copy Clips Only, No Controller)")]
        private static void SetupClipsOnly()
        {
            EnsureFolder(OutputFolder);
            int created = 0;

            foreach (SetupEntry entry in DefaultEntries)
            {
                AnimationClip clip = CreateWritableClipWithEvents(entry.SourceFbx, entry.OutputFileName);
                if (clip != null)
                    created++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[Dungeon] Created/updated {created} clip(s). Assign them manually in the Animator window.");
        }

        private static bool ProcessEntry(SetupEntry entry)
        {
            AnimationClip clip = CreateWritableClipWithEvents(entry.SourceFbx, entry.OutputFileName);
            if (clip == null)
                return false;

            if (!string.IsNullOrEmpty(entry.ControllerPath) && !string.IsNullOrEmpty(entry.StateName))
                AssignClipToState(entry.ControllerPath, entry.StateName, clip);

            return true;
        }

        private static AnimationClip CreateWritableClipWithEvents(string fbxPath, string outputFileName)
        {
            AnimationClip source = LoadFirstClipFromFbx(fbxPath);
            if (source == null)
            {
                Debug.LogWarning($"[Dungeon] No AnimationClip found in {fbxPath}");
                return null;
            }

            string outPath = $"{OutputFolder}/{outputFileName}";
            AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(outPath);

            AnimationClip writable = existing != null ? existing : new AnimationClip();
            EditorUtility.CopySerialized(source, writable);
            writable.name = System.IO.Path.GetFileNameWithoutExtension(outputFileName);

            float hitTime = Mathf.Max(0.02f, writable.length * HitNormalizedTime);
            float endTime = Mathf.Max(hitTime + 0.02f, writable.length * EndNormalizedTime);

            AnimationEvent[] events =
            {
                new AnimationEvent { time = hitTime, functionName = "OnAttackHit", stringParameter = "" },
                new AnimationEvent { time = endTime, functionName = "OnAttackEnd", stringParameter = "" }
            };

            AnimationUtility.SetAnimationEvents(writable, events);

            if (existing == null)
                AssetDatabase.CreateAsset(writable, outPath);
            else
                EditorUtility.SetDirty(writable);

            Debug.Log($"[Dungeon] {outPath} — OnAttackHit @ {hitTime:F2}s, OnAttackEnd @ {endTime:F2}s (length {writable.length:F2}s)");
            return writable;
        }

        private static AnimationClip LoadFirstClipFromFbx(string assetPath)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            return assets
                .OfType<AnimationClip>()
                .FirstOrDefault(c => !c.name.StartsWith("__"));
        }

        private static void AssignClipToState(string controllerPath, string stateName, AnimationClip clip)
        {
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null)
            {
                Debug.LogWarning($"[Dungeon] AnimatorController not found: {controllerPath}");
                return;
            }

            bool assigned = false;
            foreach (AnimatorControllerLayer layer in controller.layers)
            {
                if (AssignInStateMachine(layer.stateMachine, stateName, clip))
                    assigned = true;
            }

            if (assigned)
            {
                EditorUtility.SetDirty(controller);
                Debug.Log($"[Dungeon] Assigned {clip.name} to {stateName} in {controllerPath}");
            }
            else
            {
                Debug.LogWarning($"[Dungeon] State '{stateName}' not found in {controllerPath}. Drag {clip.name} onto the state manually.");
            }
        }

        private static bool AssignInStateMachine(AnimatorStateMachine machine, string stateName, AnimationClip clip)
        {
            bool assigned = false;

            foreach (ChildAnimatorState child in machine.states)
            {
                if (child.state.name == stateName)
                {
                    child.state.motion = clip;
                    assigned = true;
                }
            }

            foreach (ChildAnimatorStateMachine childMachine in machine.stateMachines)
            {
                if (AssignInStateMachine(childMachine.stateMachine, stateName, clip))
                    assigned = true;
            }

            return assigned;
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

        private readonly struct SetupEntry
        {
            public readonly string SourceFbx;
            public readonly string OutputFileName;
            public readonly string ControllerPath;
            public readonly string StateName;

            public SetupEntry(string sourceFbx, string outputFileName, string controllerPath, string stateName)
            {
                SourceFbx = sourceFbx;
                OutputFileName = outputFileName;
                ControllerPath = controllerPath;
                StateName = stateName;
            }
        }
    }
}
#endif
