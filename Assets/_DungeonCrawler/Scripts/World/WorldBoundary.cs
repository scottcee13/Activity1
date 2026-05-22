using UnityEngine;

namespace DungeonCrawler.World
{
    /// <summary>
    /// Invisible box colliders around the playable area. Add once per scene.
    /// </summary>
    public class WorldBoundary : MonoBehaviour
    {
        [SerializeField] private Vector3 center = Vector3.zero;
        [SerializeField] private Vector3 size = new Vector3(400f, 40f, 400f);
        [SerializeField] private float wallThickness = 2f;
        [SerializeField] private float wallHeight = 30f;
        [SerializeField] private bool buildOnAwake = true;

        private void Awake()
        {
            if (buildOnAwake)
                BuildWalls();
        }

        public void BuildWalls()
        {
            ClearChildren();

            float halfX = size.x * 0.5f;
            float halfZ = size.z * 0.5f;
            float y = center.y + wallHeight * 0.5f;

            CreateWall("Boundary_North", center + new Vector3(0f, y, halfZ), new Vector3(size.x + wallThickness * 2f, wallHeight, wallThickness));
            CreateWall("Boundary_South", center + new Vector3(0f, y, -halfZ), new Vector3(size.x + wallThickness * 2f, wallHeight, wallThickness));
            CreateWall("Boundary_East", center + new Vector3(halfX, y, 0f), new Vector3(wallThickness, wallHeight, size.z + wallThickness * 2f));
            CreateWall("Boundary_West", center + new Vector3(-halfX, y, 0f), new Vector3(wallThickness, wallHeight, size.z + wallThickness * 2f));

            CreateFloor(center + Vector3.down * 0.5f, new Vector3(size.x, 1f, size.z));
        }

        private void CreateWall(string wallName, Vector3 position, Vector3 wallSize)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(transform, false);
            wall.transform.position = position;
            wall.isStatic = true;
            wall.layer = gameObject.layer;

            BoxCollider box = wall.AddComponent<BoxCollider>();
            box.size = wallSize;
        }

        private void CreateFloor(Vector3 position, Vector3 floorSize)
        {
            GameObject floor = new GameObject("Boundary_Floor");
            floor.transform.SetParent(transform, false);
            floor.transform.position = position;
            floor.isStatic = true;
            floor.layer = gameObject.layer;

            BoxCollider box = floor.AddComponent<BoxCollider>();
            box.size = floorSize;
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(transform.GetChild(i).gameObject);
                else
                    DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
            Gizmos.DrawCube(center, size);
        }
    }
}
