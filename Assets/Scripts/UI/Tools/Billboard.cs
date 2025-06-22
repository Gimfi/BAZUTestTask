using GameCamera;
using UnityEngine;
using Zenject;

namespace UI.Tools
{
    public sealed class Billboard : MonoBehaviour
    {
        private ICameraPivot _cameraPivot;

        [Inject]
        private void Construct(ICameraPivot cameraPivot)
        {
            _cameraPivot = cameraPivot;
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - _cameraPivot.Camera.transform.position);
        }
    }
}