using Unity.Netcode;
using UnityEngine;

namespace Player.View.Spawner
{
    public sealed class PlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform _spawnPoint;

        private int _nextSpawnIndex;

        private void Start()
        {
            if (NetworkManager.Singleton)
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton)
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                playerObj.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
            }
        }
    }
}