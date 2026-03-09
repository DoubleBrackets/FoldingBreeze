using UnityEngine;

namespace Protag.Surfing
{
    [CreateAssetMenu(fileName = "SurfConfig", menuName = "Protag/SurfConfig")]
    public class SurfConfigSO : ScriptableObject
    {
        [field: Header("Config")]

        [Tooltip("Curve for mapping steering input to steering amount")]
        [field: SerializeField]
        public AnimationCurve SteeringCurve { get; private set; }

        [Tooltip("Maximum angular velocity for steering (angular velocity when steering input is 1)")]
        [field: SerializeField]
        public float SteeringMaxAngularVelocity { get; private set; }

        [Tooltip("Acceleration for horizontal movement")]
        [field: SerializeField]
        public float MoveAcceleration { get; private set; }

        [field: Tooltip("MoveAcceleration applies up to this value")]
        [field: SerializeField]
        public float MaxMoveSpeed { get; private set; }

        [field: Tooltip("Maximum horizontal speed the rigidbody is capped at")]
        [field: SerializeField]
        public float MoveSpeedCap { get; private set; }

        [field: Tooltip("Minimum horizontal speed the rigidbody is allowed to have")]
        [field: SerializeField]
        public float MinMoveSpeed { get; private set; }

        [field: Tooltip("Acceleration due to gravity when grounded")]
        [field: SerializeField]
        public float GravityAccelGround { get; private set; }

        [field: Tooltip("Acceleration due to gravity when airborne")]
        [field: SerializeField]
        public float GravityAccelAir { get; private set; }

        [field:
            Tooltip(
                "Ratio of vertical impact velocity that is converted to horizontal movement (i.e landing from a fall)")]
        [field: SerializeField]
        public float VerticalImpactBoostRatio { get; private set; }

        [field: Tooltip("Clamp range of vertical impact velocity that is converted to horizontal movement")]
        [field: SerializeField]
        public Vector2 VerticalImpactBoostRange { get; private set; }
    }
}