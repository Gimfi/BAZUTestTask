using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Network
{
    public sealed class EnemyController : NetworkBehaviour
    {
        [SerializeField]
        private NavMeshAgent _agent;

        [SerializeField]
        private float _rangeX;

        [SerializeField]
        private float _rangeZ;

        [SerializeField]
        private EnemyShootingController _shootingController;

        private Vector3 _spawnPoint;
        private Vector3 _targetPosition;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _spawnPoint = transform.position;
                AddListeners();

                CancellationToken ct = this.GetCancellationTokenOnDestroy();
                PatrolLoopAsync(ct).Forget();
            }
        }

        private void AddListeners()
        {
            _shootingController.OnTargetFound += ProcessTargetFound;
            _shootingController.OnTargetLost += ProcessTargetLost;
        }

        public override void OnNetworkDespawn()
        {
            _shootingController.OnTargetFound -= ProcessTargetFound;
            _shootingController.OnTargetLost -= ProcessTargetLost;
        }

        private async UniTask PatrolLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (!_agent.isStopped)
                {
                    await UniTask.WaitUntil(() => _agent.isActiveAndEnabled && _agent.remainingDistance < 0.5f,
                        cancellationToken: ct);

                    if (!ct.IsCancellationRequested)
                        SetRandomDestination();
                }
                else
                {
                    await UniTask.WaitUntil(() => _agent.isActiveAndEnabled && !_agent.isStopped,
                        cancellationToken: ct);
                }
            }
        }

        private void SetRandomDestination()
        {
            float randomX = Random.Range(-_rangeX, _rangeX);
            float randomZ = Random.Range(-_rangeZ, _rangeZ);

            Vector3 targetPosition = new Vector3(_spawnPoint.x + randomX, 0, _spawnPoint.z + randomZ);

            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                _targetPosition = hit.position;
                _agent.SetDestination(_targetPosition);
            }
        }

        private void ProcessTargetFound(NetworkObject _)
        {
            _agent.isStopped = true;
        }

        private void ProcessTargetLost()
        {
            _agent.isStopped = false;
        }
    }
}