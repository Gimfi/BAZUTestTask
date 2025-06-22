using System;
using UnityEngine;

namespace Input.Services
{
    public interface IInputService
    {
        event Action<Vector2> OnMoveInput;
        event Action<Vector2> OnLookInput;
    }
}