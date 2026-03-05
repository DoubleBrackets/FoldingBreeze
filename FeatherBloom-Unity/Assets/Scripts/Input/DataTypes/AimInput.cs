using UnityEngine;

namespace Input.DataTypes
{
    public struct AimInput
    {
        public Vector2 FinalAimInput;
        public Quaternion ProcessedFanOrientation;
        public Quaternion RawFanOrientation;
        public Vector3 PhysicalForwardAxis;

        public AimInput(Vector2 finalAimInput,
            Quaternion processedFanOrientation = default,
            Quaternion rawFanOrientation = default,
            Vector3 physicalForwardAxis = default)
        {
            FinalAimInput = finalAimInput;
            ProcessedFanOrientation = processedFanOrientation;
            RawFanOrientation = rawFanOrientation;
            PhysicalForwardAxis = physicalForwardAxis;
        }

        public AimInput(Vector2 finalAimInput)
        {
            FinalAimInput = finalAimInput;
            ProcessedFanOrientation = Quaternion.identity;
            RawFanOrientation = Quaternion.identity;
            PhysicalForwardAxis = Vector3.forward;
        }
    }
}