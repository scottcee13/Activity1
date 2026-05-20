using System.Collections.Generic;
using UnityEngine;

namespace DungeonCrawler.Boss
{
    [System.Serializable]
    public class SpawnWave
    {
        public GameObject enemyPrefab;
        public int count = 3;
    }

    /// <summary>
    /// Spawns enemy waves at configured points. Used in boss room and combat rooms.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
        [SerializeField] private List<SpawnWave> waves = new List<SpawnWave>();
        [SerializeField] private int maxAlive = 10;

        private readonly List<GameObject> alive = new List<GameObject>();

        public void SpawnWave(int waveIndex)
        {
            if (waveIndex < 0 || waveIndex >= waves.Count) return;

            SpawnWave wave = waves[waveIndex];
            for (int i = 0; i < wave.count; i++)
            {
                if (alive.Count >= maxAlive) break;
                if (spawnPoints.Count == 0 || wave.enemyPrefab == null) break;

                Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                GameObject enemy = Instantiate(wave.enemyPrefab, point.position, point.rotation);
                alive.Add(enemy);

                Combat.HealthComponent h = enemy.GetComponent<Combat.HealthComponent>();
                if (h != null)
                    h.OnDeath += () => alive.Remove(enemy);
            }
        }

        public void ClearAll()
        {
            foreach (GameObject e in alive)
            {
                if (e != null) Destroy(e);
            }
            alive.Clear();
        }
    }
}
