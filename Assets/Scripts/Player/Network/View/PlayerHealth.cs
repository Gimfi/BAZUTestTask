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
            if (IsServer)
                _currentHealth.Value = _maxHealth;

            ShowHp(_currentHealth.Value);
            _currentHealth.OnValueChanged += OnValueChanged;
        }

        public override void OnNetworkDespawn()
        {
            _currentHealth.OnValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(int previousValue, int newValue)
        {
            ShowHp(newValue);
        }

        private void ShowHp(int value)
        {
            _hpText.text = value > 0 ? value.ToString() : "Zombie";
        }

        public void TakeDamage(int damage)
        {
            if (IsServer)
            {
                damage = Mathf.Max(damage, 0);
                _currentHealth.Value = Mathf.Max(_currentHealth.Value - damage, 0);
            }
        }
    }
}