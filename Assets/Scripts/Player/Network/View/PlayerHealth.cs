using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Player.Network.View
{
    public sealed class PlayerHealth : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text _hpText;

        [Header("Configs")]
        [SerializeField]
        private int _maxHealth = 100;

        private readonly NetworkVariable<int> _currentHealth = new();

        public int CurrentHealth => _currentHealth.Value;

        public override void OnNetworkSpawn()
        {
            _currentHealth.Value = _maxHealth;
            _hpText.text = _currentHealth.Value.ToString();

            _currentHealth.OnValueChanged += OnValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            _currentHealth.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(int previousValue, int newValue)
        {
            _hpText.text = newValue > 0 ? newValue.ToString() : "Zombie";
        }

        public void TakeDamage(int damage)
        {
            if (IsServer)
            {
                _currentHealth.Value -= damage;
                _currentHealth.Value = Mathf.Max(_currentHealth.Value, 0);
            }
        }
    }
}