#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DungeonCrawler.Editor
{
    /// <summary>
    /// Tools → Dungeon → Add Environment Colliders
    /// Bulk-adds BoxColliders to static mesh children without colliders.
    /// </summary>
    public static class EnvironmentColliderUtility
    {
        [MenuItem("Tools/Dungeon/Add Box Colliders To Selected (Static)")]
        private static void AddBoxCollidersToSelection()
        {
            int added = 0;
            foreach (GameObject root in Selection.gameObjects)
            {
                added += ProcessHierarchy(root.transform, false);
            }
            Debug.Log($"[Dungeon] Added {added} BoxCollider(s). Mark roots Static for best performance.");
        }

        [MenuItem("Tools/Dungeon/Add Mesh Colliders To Selected (Convex)")]
        private static void AddMeshCollidersConvex()
        {
            int added = 0;
            foreach (GameObject root in Selection.gameObjects)
            {
                added += ProcessHierarchy(root.transform, true);
            }
            Debug.Log($"[Dungeon] Added {added} convex MeshCollider(s).");
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

                if (useMeshCollider)
                {
                    MeshCollider mc = Undo.AddComponent<MeshCollider>(go);
                    mc.convex = false;
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
