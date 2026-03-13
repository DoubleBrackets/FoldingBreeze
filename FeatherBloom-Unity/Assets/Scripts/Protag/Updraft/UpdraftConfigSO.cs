using UnityEngine;

namespace Protag.Updraft
{
    [CreateAssetMenu(fileName = "UpdraftConfig", menuName = "Protag/UpdraftConfig")]
    public class UpdraftConfigSO : ScriptableObject
    {
        [field: Header("Config")]

        [field: SerializeField]
        public float FeathersConsumed { get; private set; }

        [field: SerializeField]
        public float UpdraftVelocity { get; private set; }

        [field: SerializeField]
        public AnimationCurve UpdraftVelocityCurve { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public float HorizontalVelocityKeepRatio { get; private set; }

        [field: SerializeField]
        public float TimeSlowdownBlockDuration { get; private set; }
    }
}