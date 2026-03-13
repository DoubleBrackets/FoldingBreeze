using Input.DataTypes;
using UI;
using UnityEngine;

namespace Input.Processor
{
    /// <summary>
    ///     An extra processing layer on top of plain input. The main usage is to handle the issue where making fan gestures
    ///     throws off aim. This is handled by "untracking" input when the fan is too far away from forward (a crappy heuristic
    ///     for when the user is trying to gesture), then restarting tracking when the fan is close enough to forward again,
    ///     indicating gesture is done.
    ///     TODO: Hook up the timeScale such that post-gesture slowdown continues until tracking restarts
    ///     TODO: Maybe have gestures force stop tracking
    /// </summary>
    public class CustomControllerInputProcessor
    {
        private CustomFanInputConfigSO _config;

        private InputProcessorState _state = InputProcessorState.Tracking;

        public InputProcessorState State => _state;

        public CustomControllerInputProcessor(CustomFanInputConfigSO config)
        {
            _config = config;
        }

        public ProcessResult ProcessInput(GameplayInputType scheme, AimInput aim)
        {
            InputProcessorState currentState = _state;
            Vector3 forward = aim.ProcessedFanOrientation * aim.PhysicalForwardAxis;
            float angleFromForward = Vector3.Angle(forward, Vector3.forward);

            var result = new ProcessResult
            {
                CurrentFanForward = forward,
                CurrentFanAngleFromForward = angleFromForward,
                PreviousState = _state,
                ProcessedFanOrientation = aim.ProcessedFanOrientation
            };
            if (scheme == GameplayInputType.Conventional)
            {
                result.ProcessedAimInput = aim.FinalAimInput;
                result.CurrentState = InputProcessorState.Tracking;
                return result;
            }

            if (_state == InputProcessorState.Untracked)
            {
                if (angleFromForward < _config.FanRestartTrackingAngle)
                {
                    _state = InputProcessorState.Tracking;
                }

                result.ProcessedAimInput = Vector2.zero;
            }
            else
            {
                if (angleFromForward > _config.FanLoseTrackingAngle)
                {
                    // _state = InputProcessorState.Untracked;
                    result.ProcessedAimInput = Vector2.zero;
                }
                else if (angleFromForward < _config.FanDeadZoneAngle)
                {
                    // result.ProcessedAimInput = Vector2.zero;
                }
                else
                {
                    result.ProcessedAimInput = aim.FinalAimInput;
                }
            }

            result.CurrentState = _state;
            return result;
        }
    }
}