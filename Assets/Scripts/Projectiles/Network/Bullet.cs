using System.Threading;
using Cysharp.Threading.Tasks;
using Player.Network.View;
using Unity.Netcode;
using UnityEngine;

namespace Projectiles.Network
{
    public sealed class Bullet : NetworkBehaviour
    {
        [SerializeField]
        private float _speed;

        [SerializeField]
        private int _damage;

        [SerializeField]
        private float _detectionRadius;

        private Vector3 _direction;
        private readonly Collider[] _overlapSphereResults = new Collider[5];

        public void Init(Vector3 direction)
        {
            if (IsServer)
            {
                _direction = direction.normalized;

                CancellationToken ct = this.GetCancellationTokenOnDestroy();
                BulletFly(ct).Forget();
                FindBulletCollision(ct).Forget();
            }
        }

        private async UniTask BulletFly(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                transform.position += _direction * (_speed * Time.deltaTime);
                await UniTask.Yield();
            }
        }

        private async UniTask FindBulletCollision(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                int size = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _overlapSphereResults);
                if (size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (_overlapSphereResults[i].gameObject.TryGetComponent(out PlayerHealth playerHealth))
                            playerHealth.TakeDamage(_damage);
                    }

                    GetComponent<NetworkObject>().Despawn();
                }

                await UniTask.Yield();
            }
        }
    }
}