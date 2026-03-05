using UnityEngine;

namespace Input.Processor
{
    public struct ProcessResult
    {
        public Vector2 ProcessedAimInput;
        public Quaternion ProcessedFanOrientation;
        public Vector3 CurrentFanForward;
        public float CurrentFanAngleFromForward;
        public InputProcessorState PreviousState;
        public InputProcessorState CurrentState;

        public bool DidBeginTracking => PreviousState != CurrentState && CurrentState == InputProcessorState.Tracking;
        public bool DidEndTracking => PreviousState != CurrentState && CurrentState == InputProcessorState.Untracked;
    }
}