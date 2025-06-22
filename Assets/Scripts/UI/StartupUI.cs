using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public sealed class StartupUI : MonoBehaviour
    {
        [SerializeField]
        private Button _hostButton;

        [SerializeField]
        private Button _clientButton;

        [SerializeField]
        private Button _shutdownButton;

        private void Awake()
        {
            _hostButton.onClick.AddListener(StartHost);
            _clientButton.onClick.AddListener(StartClient);
            _shutdownButton.onClick.AddListener(Shutdown);
        }

        private void OnDestroy()
        {
            _hostButton.onClick.RemoveListener(StartHost);
            _clientButton.onClick.RemoveListener(StartClient);
            _shutdownButton.onClick.RemoveListener(Shutdown);
        }

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        private void Shutdown()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}