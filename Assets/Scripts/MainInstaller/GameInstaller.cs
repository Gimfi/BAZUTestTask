using Input.Installers;
using Zenject;

namespace MainInstaller
{
    public sealed class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            InputInstaller.Install(Container);
        }
    }
}