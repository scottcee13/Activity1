#if UNITY_EDITOR
using DungeonCrawler.World;
using UnityEngine;
using UnityEditor;

namespace DungeonCrawler.Editor
{
    /// <summary>
    /// Tools → Dungeon → environment colliders and world bounds.
    /// </summary>
    public static class EnvironmentColliderUtility
    {
        [MenuItem("Tools/Dungeon/Add Box Colliders To Selected (Static)")]
        private static void AddBoxCollidersToSelection()
        {
            int added = 0;
            foreach (GameObject root in Selection.gameObjects)
                added += ProcessHierarchy(root.transform, false);

            Debug.Log($"[Dungeon] Added {added} BoxCollider(s). Mark roots Static for best performance.");
        }

        [MenuItem("Tools/Dungeon/Add Mesh Colliders To Selected (Convex)")]
        private static void AddMeshCollidersConvex()
        {
            int added = 0;
            foreach (GameObject root in Selection.gameObjects)
                added += ProcessHierarchy(root.transform, true);

            Debug.Log($"[Dungeon] Added {added} convex MeshCollider(s).");
        }

        [MenuItem("Tools/Dungeon/Create World Boundary From Selection")]
        private static void CreateWorldBoundaryFromSelection()
        {
            Bounds bounds = new Bounds(Selection.activeTransform.position, Vector3.one);
            bool init = false;

            foreach (GameObject go in Selection.gameObjects)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    if (!init)
                    {
                        bounds = r.bounds;
                        init = true;
                    }
                    else
                        bounds.Encapsulate(r.bounds);
                }
            }

            if (!init)
            {
                Debug.LogWarning("[Dungeon] Select environment objects to compute bounds.");
                return;
            }

            GameObject boundaryRoot = new GameObject("WorldBoundary");
            Undo.RegisterCreatedObjectUndo(boundaryRoot, "Create World Boundary");

            WorldBoundary boundary = boundaryRoot.AddComponent<WorldBoundary>();
            SerializedObject so = new SerializedObject(boundary);
            so.FindProperty("center").vector3Value = bounds.center;
            so.FindProperty("size").vector3Value = bounds.size + new Vector3(20f, 0f, 20f);
            so.FindProperty("buildOnAwake").boolValue = true;
            so.ApplyModifiedPropertiesWithoutUndo();

            boundary.BuildWalls();
            Debug.Log($"[Dungeon] World boundary created. Center={bounds.center}, Size={bounds.size}");
        }

        [MenuItem("Tools/Dungeon/Mark Selected Environment Static")]
        private static void MarkStatic()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
                {
                    Undo.RecordObject(t.gameObject, "Mark Static");
                    t.gameObject.isStatic = true;
                }
            }
        }

        private static int ProcessHierarchy(Transform root, bool useMeshCollider)
        {
            int count = 0;
            MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>(true);
            foreach (MeshFilter mf in filters)
            {
                if (mf.sharedMesh == null) continue;

                GameObject go = mf.gameObject;
                if (go.GetComponent<Collider>() != null) continue;
                if (go.CompareTag("Player")) continue;
                if (go.layer == LayerMask.NameToLayer("Enemy")) continue;

                if (useMeshCollider)
                {
                    MeshCollider mc = Undo.AddComponent<MeshCollider>(go);
                    mc.convex = true;
                    mc.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation;
                }
                else
                {
                    BoxCollider box = Undo.AddComponent<BoxCollider>(go);
                    Bounds b = mf.sharedMesh.bounds;
                    box.center = b.center;
                    box.size = b.size;
                }

                Undo.RecordObject(go, "Mark Static");
                go.isStatic = true;
                count++;
            }
            return count;
        }
    }
}
#endif
