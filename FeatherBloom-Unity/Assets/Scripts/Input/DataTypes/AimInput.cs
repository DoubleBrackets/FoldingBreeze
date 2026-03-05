using UnityEngine;

namespace Input.DataTypes
{
    public struct AimInput
    {
        public Vector2 FinalAimInput;
        public Quaternion ProcessedFanOrientation;
        public Quaternion RawFanOrientation;
    }
}