using UnityEngine;
using Zenject;

namespace GameCamera.Installers
{
    public sealed class CameraInstaller : MonoInstaller<CameraInstaller>
    {
        [SerializeField]
        private CameraPivot _cameraPivot;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<CameraPivot>().FromInstance(_cameraPivot);
        }
    }
}