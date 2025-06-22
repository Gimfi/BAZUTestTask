using System;
using UnityEngine;

namespace Input.Services
{
    public sealed class InputService : IInputService
    {
        public event Action<Vector2> OnMoveInput;
        public event Action<Vector2> OnLookInput;

        private readonly PlayerInputActions _inputActions = new();

        public InputService()
        {
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += ctx => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
            _inputActions.Player.Move.canceled += _ => OnMoveInput?.Invoke(Vector2.zero);

            _inputActions.Player.Look.performed += ctx => OnLookInput?.Invoke(ctx.ReadValue<Vector2>());
            _inputActions.Player.Look.canceled += _ => OnLookInput?.Invoke(Vector2.zero);
        }

        ~InputService()
        {
            _inputActions.Disable();
        }
    }
}