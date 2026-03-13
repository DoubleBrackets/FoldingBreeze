using UnityEngine;

namespace Protag.Gliding
{
    [CreateAssetMenu(fileName = "GlideConfig", menuName = "Protag/GlideConfig")]
    public class GlideConfigSO : ScriptableObject
    {
        [field: Header("Other")]

        [field: SerializeField]
        public float FeatherConsumptionPerSecond { get; private set; }

        [field: Header("Input Mapping")]

        [field: SerializeField]
        public AnimationCurve TiltSteerCurve { get; private set; }

        [field: SerializeField]
        public AnimationCurve RollSteerCurve { get; private set; }

        [field: Header("Physics")]

        [field: SerializeField]
        public float TiltMaxAngularVelocity { get; private set; }

        [field: SerializeField]
        public float RollMaxAngularVelocity { get; private set; }

        [field: SerializeField]
        public float GravityAccel { get; private set; }

        [field: SerializeField]
        public float FixedGravityAccel { get; private set; }

        [field: SerializeField]
        public float Drag { get; private set; }

        [field: SerializeField]
        public float MinFlightSpeed { get; private set; }

        [field: SerializeField]
        public float TiltBoundUpper { get; private set; }

        [field: SerializeField]
        public float TiltBoundLower { get; private set; }
    }
}