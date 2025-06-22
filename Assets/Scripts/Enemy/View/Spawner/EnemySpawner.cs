using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.View.Spawner
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkObject _enemyPrefab;

        [SerializeField]
        private List<Transform> _spawnPoints = new();

        [Header("Configs")]
        [SerializeField]
        private int _maxEnemyCount;

        [SerializeField]
        private int _spawnDelay;

        private bool _isSpawning;
        private int _nextSpawnIndex;
        private readonly List<NetworkObject> _enemies = new();

        private void Start()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnServerStarted += StartSpawnEnemies;
                NetworkManager.Singleton.OnServerStopped += StopSpawnEnemies;
            }
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnServerStarted -= StartSpawnEnemies;
                NetworkManager.Singleton.OnServerStopped -= StopSpawnEnemies;
            }
        }

        private void StartSpawnEnemies()
        {
            _enemies.Clear();
            _isSpawning = true;
            SpawnLoopAsync().Forget();
        }

        private void StopSpawnEnemies(bool _)
        {
            _enemies.Clear();
            _isSpawning = false;
        }

        private async UniTask SpawnLoopAsync()
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            while (!ct.IsCancellationRequested && _enemies.Count < _maxEnemyCount && _isSpawning)
            {
                SpawnEnemy();
                await UniTask.Delay(_spawnDelay, cancellationToken: ct);
            }
        }

        private void SpawnEnemy()
        {
            Vector3 spawnPos = GetRandomSpawnPoint();

            NetworkObject enemy = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(_enemyPrefab,
                position: spawnPos,
                rotation: Quaternion.identity);

            _enemies.Add(enemy);
        }

        private Vector3 GetRandomSpawnPoint()
        {
            int randomIndex = Random.Range(0, _spawnPoints.Count);
            return _spawnPoints[randomIndex].position;
        }
    }
}