using GameCamera;
using Input.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Player.Network.View
{
    public sealed class PlayerController : NetworkBehaviour
    {
        private readonly Vector3 _noMoveVector = Vector3.zero;

        [SerializeField]
        private float _speed;

        private IInputService _inputService;
        private ICameraPivot _cameraPivot;
        private Vector2 _moveVector;

        [Inject]
        private void Construct(IInputService inputService, ICameraPivot cameraPivot)
        {
            _inputService = inputService;
            _cameraPivot = cameraPivot;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _inputService.OnMoveInput += UpdateMoveVector;
                _cameraPivot.SetTarget(transform);
            }
            else
            {
                enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                _cameraPivot.LeftTarget();
                _inputService.OnMoveInput -= UpdateMoveVector;
            }
        }

        private void UpdateMoveVector(Vector2 moveVector)
        {
            _moveVector = moveVector;
        }

        private void Update()
        {
            if (IsOwner)
            {
                Vector3 moveDir = GetMoveDirection();
                MoveServerRpc(moveDir);
            }
        }

        private Vector3 GetMoveDirection()
        {
            if (_moveVector.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = _cameraPivot.Camera.transform.forward;
                camForward.y = 0;
                camForward.Normalize();

                Vector3 camRight = _cameraPivot.Camera.transform.right;
                camRight.y = 0;
                camRight.Normalize();

                Vector3 moveDir = camForward * _moveVector.y + camRight * _moveVector.x;
                moveDir.Normalize();

                return moveDir;
            }

            return _noMoveVector;
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 direction)
        {
            transform.position += direction * _speed * Time.deltaTime;
        }
    }
}