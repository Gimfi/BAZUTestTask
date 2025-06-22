using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Player.Network.View;
using Projectiles.Network;
using Unity.Netcode;
using UnityEngine;

namespace Enemy.Network
{
    [RequireComponent(typeof(EnemyController))]
    public sealed class EnemyShootingController : NetworkBehaviour
    {
        [SerializeField]
        private NetworkObject _bulletPrefab;

        [SerializeField]
        private Transform _shootPoint;

        [Header("Configs")]
        [SerializeField]
        private float _detectionRange;

        [SerializeField]
        private int _scanInterval;

        [SerializeField]
        private Vector2 _bulletForce;

        [SerializeField]
        private int _shootInterval;

        private NetworkObject _target;
        public event Action<NetworkObject> OnTargetFound;
        public event Action OnTargetLost;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                CancellationToken ct = this.GetCancellationTokenOnDestroy();
                FindClosestPlayerLoop(ct).Forget();
            }
        }

        private async UniTask FindClosestPlayerLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(_scanInterval, cancellationToken: ct);

                if (!ct.IsCancellationRequested)
                {
                    _target = FindClosestPlayer();

                    if (_target)
                    {
                        OnTargetFound?.Invoke(_target);
                        FocusOnTarget(ct).Forget();
                        await ShootingLoop(ct);
                    }
                }
            }
        }

        private NetworkObject FindClosestPlayer()
        {
            float minDist = _detectionRange;
            NetworkObject closest = null;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                NetworkObject playerObj = client.PlayerObject;
                if (IsTargetValid(playerObj))
                {
                    float dist = Vector3.Distance(transform.position, playerObj.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = playerObj;
                    }
                }
            }

            return closest;
        }

        private async UniTask ShootingLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                float dist = Vector3.Distance(transform.position, _target.transform.position);
                if (dist < _detectionRange && IsTargetValid(_target))
                {
                    Vector3 targetPos = _target.transform.position;
                    targetPos.y += 1;

                    Vector3 dir = (targetPos - _shootPoint.position).normalized;
                    Shoot(dir);

                    await UniTask.Delay(_shootInterval, cancellationToken: ct);
                }
                else
                {
                    _target = null;
                    OnTargetLost?.Invoke();
                    break;
                }
            }
        }

        private void Shoot(Vector3 direction)
        {
            NetworkObject bullet = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(_bulletPrefab,
                position: _shootPoint.position,
                rotation: Quaternion.LookRotation(direction));

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Init(direction);
        }

        private async UniTask FocusOnTarget(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _target)
            {
                Vector3 lookDir = _target.transform.position - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
        }

        private bool IsTargetValid(NetworkObject target)
        {
            return target && target.gameObject.TryGetComponent(out PlayerHealth playerHealth) &&
                   playerHealth.CurrentHealth > 0;
        }
    }
}