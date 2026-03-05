using Input.DataTypes;
using Input.SerialComms;
using NaughtyAttributes;
using UnityEngine;
using ValueSO.Core;

namespace Input.FanInput
{
    /// <summary>
    ///     Input provider for hand fan arduino. Handles translating raw input data to gameplay input
    /// </summary>
    public class HandFanInputProvider : InputProvider
    {
        [Header("ValueSO (Read)")]

        [SerializeField]
        private QuaternionValueSO _defaultOrientation;

        [Header("Config")]

        [SerializeField]
        private float _closeSensitivity;

        [SerializeField]
        private float _openTiltSensitivity;

        [SerializeField]
        private float _openRollSensitivity;

        [SerializeField]
        private Vector3 _fanOpenRollForwardAxis;

        [InfoBox("The axis of the fan that is pointing towards the screen when open")]
        [SerializeField]
        private Vector3 _fanOpenPhysicalForwardAxis;

        [SerializeField]
        private Vector3 _fanOpenTiltForwardAxis;

        [SerializeField]
        private FanGestureRecognizer.GestureRecognizeConfig _gestureConfig;

        private Quaternion ZeroedRawOrientation => _defaultOrientation.Value;

        private Quaternion _lastRawOrientation;

        private FanState _currentFanState = FanState.Closed;
        private Vector2 _lastAimInput;
        private bool _fanOpenSwitchState;

        private FanGestureRecognizer _gestureRecognizer;

        private void Awake()
        {
            _gestureRecognizer = new FanGestureRecognizer(_gestureConfig);
            _gestureRecognizer.OnGestureTriggered += HandleGestureRecognized;
        }

        private void OnDestroy()
        {
            _gestureRecognizer.OnGestureTriggered -= HandleGestureRecognized;
        }

        private void HandleGestureRecognized(FanGestureRecognizer.GestureTypes type)
        {
            Debug.Log($"{type.ToString()} Gestured");

            // Prevent accidently slicing when flicking the wrist to close
            /*if (!_fanOpenSwitchState)
            {
                return;
            }
            */

            if (_currentFanState == FanState.Closed)
            {
                return;
            }

            switch (type)
            {
                case FanGestureRecognizer.GestureTypes.Gust:
                    GustInput?.Invoke();
                    break;
                case FanGestureRecognizer.GestureTypes.Slice:
                    SliceInput?.Invoke();
                    break;
                case FanGestureRecognizer.GestureTypes.Updraft:
                    UpdraftInput?.Invoke();
                    break;
                case FanGestureRecognizer.GestureTypes.FanSelf:
                    FanSelfInput?.Invoke();
                    break;
            }
        }

        public void HandleSerialReadResult(HandFanArduinoComm.SerialReadResult result)
        {
            _fanOpenSwitchState = result.OpenFanSwitch;
            if (result.OpenFanSwitch)
            {
                _currentFanState = FanState.Open;
                DesiredFanStateChanged?.Invoke(FanState.Open);
            }
            else if (result.CloseFanSwitch)
            {
                _currentFanState = FanState.Closed;
                DesiredFanStateChanged?.Invoke(FanState.Closed);
            }

            bool inTransition = !result.OpenFanSwitch && !result.CloseFanSwitch;

            Quaternion rawOrientation = result.Orientation;
            _lastRawOrientation = rawOrientation;

            Quaternion transformedOrientation = ConvertRawToDefaulted(rawOrientation);

            Vector2 aimInput = _currentFanState == FanState.Open
                ? ConvertOrientationToAimOpen(transformedOrientation)
                : ConvertOrientationToAimClosed(transformedOrientation);

            // If in transition, use last aim input
            /*if (inTransition)
            {
                aimInput = _lastAimInput;
            }*/

            _lastAimInput = aimInput;

            AimInputChanged?.Invoke(new AimInput(
                aimInput,
                transformedOrientation,
                rawOrientation,
                _currentFanState == FanState.Open ? _fanOpenPhysicalForwardAxis : Vector3.forward
            ));

            _gestureRecognizer.AddGesturePoint(transformedOrientation, Time.realtimeSinceStartup);
            _gestureRecognizer.ProcessGestures();
        }

        private Quaternion ConvertRawToDefaulted(Quaternion rawFanOrientation)
        {
            // Apply default orientation
            rawFanOrientation = Quaternion.Inverse(ZeroedRawOrientation) * rawFanOrientation;

            // Transform axis
            rawFanOrientation = new Quaternion(rawFanOrientation.y, rawFanOrientation.z, -rawFanOrientation.x,
                -rawFanOrientation.w);

            return rawFanOrientation;
        }

        /// <summary>
        ///     Converts the raw fan orientation to an aim input for open fan state
        /// </summary>
        /// <param name="fanOrientation"></param>
        /// <returns></returns>
        private Vector2 ConvertOrientationToAimOpen(Quaternion fanOrientation)
        {
            Vector3 rollDir = fanOrientation * _fanOpenRollForwardAxis;
            // Project onto YZ plane to get roll
            float rollProjected = -rollDir.y;

            Vector3 tiltDir = fanOrientation * _fanOpenTiltForwardAxis;
            // Project onto XY plane to get tilt
            float tiltProjected = tiltDir.y;

            // roll is x input dir, tilt is y input dir
            var projected = new Vector2(rollProjected, tiltProjected);
            projected.x *= _openRollSensitivity;
            projected.y *= _openTiltSensitivity;
            projected.x = Mathf.Clamp(projected.x, -1f, 1f);
            projected.y = Mathf.Clamp(projected.y, -1f, 1f);

            return projected;
        }

        /// <summary>
        ///     Converts the raw fan orientation to an aim input for closed fan state
        /// </summary>
        /// <param name="fanOrientation"></param>
        /// <returns></returns>
        private Vector2 ConvertOrientationToAimClosed(Quaternion fanOrientation)
        {
            // Roll uses up axis
            Vector3 dir = fanOrientation * Vector3.forward;

            // Closed fan needs to go backwards
            /*if (dir.z > 0)
            {
                return Vector2.zero;
            }*/

            // Project onto XZ plane to get horizontal aim direction
            var projected = new Vector2(dir.x, dir.z);
            projected.Normalize();

            // Closed mode doesn't use vertical input
            projected.y = 0;

            projected *= _closeSensitivity;
            projected.x = Mathf.Clamp(projected.x, -1f, 1f);
            projected.y = Mathf.Clamp(projected.y, -1f, 1f);

            return projected;
        }

        public override void SetDefaultToCurrent()
        {
            _defaultOrientation.SetValue(_lastRawOrientation);
        }
    }
}