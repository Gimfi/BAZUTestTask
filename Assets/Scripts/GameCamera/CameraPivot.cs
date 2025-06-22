using System.Threading;
using Cysharp.Threading.Tasks;
using Input.Services;
using UnityEngine;
using Zenject;

namespace GameCamera
{
    public sealed class CameraPivot : MonoBehaviour, ICameraPivot
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _mouseSensitivity;

        private float _yRotation;
        private Vector2 _lookInput;

        private IInputService _inputService;
        private Transform _target;

        [Inject]
        private void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        public Camera Camera => _camera;

        public void SetTarget(Transform target)
        {
            SetupTarget(target);
            _inputService.OnLookInput += UpdateLookVector;

            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            CameraRotationLoop(ct).Forget();
        }

        private void OnDestroy()
        {
            _inputService.OnLookInput -= UpdateLookVector;
        }

        public void LeftTarget()
        {
            SetupTarget(null);
            _inputService.OnLookInput -= UpdateLookVector;
        }

        private void SetupTarget(Transform target)
        {
            _target = target;
            transform.SetParent(target);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void UpdateLookVector(Vector2 lookInput)
        {
            _lookInput = lookInput;
        }

        private async UniTask CameraRotationLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && _target)
            {
                _yRotation += _lookInput.x * _mouseSensitivity;
                transform.rotation = Quaternion.Euler(0, _yRotation, 0);

                await UniTask.Yield();
            }
        }
    }
}