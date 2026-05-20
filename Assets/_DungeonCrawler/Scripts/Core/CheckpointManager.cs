using System.IO;
using UnityEngine;

namespace DungeonCrawler.Core
{
    /// <summary>
    /// Saves player position and current dungeon room at checkpoints.
    /// Works alongside legacy SaveSystem (inventory + quests).
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        [SerializeField] private string checkpointFileName = "checkpoint.json";

        public string CurrentRoomId { get; private set; }
        public bool HasCheckpoint { get; private set; }

        private string Path => System.IO.Path.Combine(Application.persistentDataPath, checkpointFileName);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            LoadCheckpoint();
        }

        public void SaveCheckpoint(Transform player, string roomId)
        {
            if (player == null) return;

            CheckpointData data = new CheckpointData
            {
                posX = player.position.x,
                posY = player.position.y,
                posZ = player.position.z,
                rotY = player.eulerAngles.y,
                roomId = roomId
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path, json);
            CurrentRoomId = roomId;
            HasCheckpoint = true;

            if (SaveSystem.instance != null)
                SaveSystem.instance.SaveGame();

            Debug.Log($"[Checkpoint] Saved at room {roomId}");
        }

        public void LoadCheckpoint()
        {
            if (!File.Exists(Path))
            {
                HasCheckpoint = false;
                return;
            }

            CheckpointData data = JsonUtility.FromJson<CheckpointData>(File.ReadAllText(Path));
            CurrentRoomId = data.roomId;
            HasCheckpoint = true;
            pendingSpawn = data;
        }

        private CheckpointData pendingSpawn;

        private void Start()
        {
            ApplyPendingSpawn();
        }

        private void ApplyPendingSpawn()
        {
            if (pendingSpawn == null) return;

            Transform player = GameManager.Instance != null
                ? GameManager.Instance.GetPlayer()
                : null;

            if (player == null)
            {
                GameObject found = GameObject.FindGameObjectWithTag("Player");
                if (found != null) player = found.transform;
            }

            if (player == null) return;

            player.position = new Vector3(pendingSpawn.posX, pendingSpawn.posY, pendingSpawn.posZ);
            player.rotation = Quaternion.Euler(0f, pendingSpawn.rotY, 0f);
            pendingSpawn = null;
        }

        public void ClearCheckpoint()
        {
            if (File.Exists(Path)) File.Delete(Path);
            HasCheckpoint = false;
            CurrentRoomId = null;
        }

        [System.Serializable]
        private class CheckpointData
        {
            public float posX, posY, posZ;
            public float rotY;
            public string roomId;
        }
    }
}
