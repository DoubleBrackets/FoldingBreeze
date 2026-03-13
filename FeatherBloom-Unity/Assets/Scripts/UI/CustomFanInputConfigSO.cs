using NaughtyAttributes;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "CustomFanInputConfig", menuName = "Fan Input Config")]
    public class CustomFanInputConfigSO : ScriptableObject
    {
        [field: InfoBox("If the physical fan is this many degrees away from forward, then stop detecting input." +
                        "This is to allow gestures to be made without causing sharp turns")]
        [field: SerializeField]
        public float FanLoseTrackingAngle { get; private set; }

        [field: InfoBox("If the physical fan is this many degrees away from forward, then restart detecting input." +
                        "This is to allow input to be reacquired after a gesture without creating sharp turns")]
        [field: SerializeField]
        public float FanRestartTrackingAngle { get; private set; }

        [field: SerializeField]
        public float FanDeadZoneAngle { get; private set; }
    }
}