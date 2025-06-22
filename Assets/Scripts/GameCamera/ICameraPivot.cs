using UnityEngine;

namespace GameCamera
{
    public interface ICameraPivot
    {
        Camera Camera { get; }

        void SetTarget(Transform transform);
        void LeftTarget();
    }
}