using Input.Services;
using Zenject;

namespace Input.Installers
{
    public sealed class InputInstaller : Installer<InputInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputService>().AsSingle();
        }
    }
}