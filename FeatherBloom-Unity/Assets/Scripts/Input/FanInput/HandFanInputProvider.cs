using SerialComms;
using UnityEngine;

namespace Input.FanInput
{
    /// <summary>
    ///     Input provider for hand fan arduino. Handles translating raw input data to gameplay input
    /// </summary>
    public class HandFanInputProvider : InputProvider
    {
        [Header("Config")]

        [SerializeField]
        private float _closeSensitivity;

        [SerializeField]
        private float _openTiltSensitivity;

        [SerializeField]
        private float _openRollSensitivity;

        [Tooltip("Deadzone, applied after sensitivity")]
        [SerializeField]
        private float _deadzone;

        [SerializeField]
        private Vector3 _fanOpenRollForwardAxis;

        [SerializeField]
        private Vector3 _fanOpenTiltForwardAxis;

        [SerializeField]
        private FanGestureRecognizer.GestureRecognizeConfig _gestureConfig;

        public static HandFanInputProvider Instance { get; private set; }

        public Quaternion ZeroedRawOrientation { get; set; }

        private Quaternion _lastRawOrientation;

        private GameplayInputService.FanState _currentFanState = GameplayInputService.FanState.Closed;
        private Vector2 _lastAimInput;
        private bool _fanOpenSwitchState;

        private FanGestureRecognizer _gestureRecognizer;

        private void Awake()
        {
            _gestureRecognizer = new FanGestureRecognizer(_gestureConfig);
            Instance = this;
            ZeroedRawOrientation = Quaternion.identity;
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
            if (!_fanOpenSwitchState)
            {
                return;
            }

            if (_currentFanState == GameplayInputService.FanState.Closed)
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
            }
        }

        public void HandleSerialReadResult(HandFanArduinoComm.SerialReadResult result)
        {
            _fanOpenSwitchState = result.OpenFanSwitch;
            if (result.OpenFanSwitch)
            {
                _currentFanState = GameplayInputService.FanState.Open;
                DesiredFanStateChanged?.Invoke(GameplayInputService.FanState.Open);
            }
            else if (result.CloseFanSwitch)
            {
                _currentFanState = GameplayInputService.FanState.Closed;
                DesiredFanStateChanged?.Invoke(GameplayInputService.FanState.Closed);
            }

            bool inTransition = !result.OpenFanSwitch && !result.CloseFanSwitch;

            Quaternion rawOrientation = result.Orientation;
            _lastRawOrientation = rawOrientation;

            Quaternion transformedOrientation = ConvertRawToDefaulted(rawOrientation);

            Vector2 aimInput = _currentFanState == GameplayInputService.FanState.Open
                ? ConvertOrientationToAimOpen(transformedOrientation)
                : ConvertOrientationToAimClosed(transformedOrientation);

            // If in transition, use last aim input
            /*if (inTransition)
            {
                aimInput = _lastAimInput;
            }*/

            _lastAimInput = aimInput;

            AimInputChanged?.Invoke(new GameplayInputService.AimInput
            {
                FinalAimInput = aimInput,
                ProcessedFanOrientation = transformedOrientation,
                RawFanOrientation = rawOrientation
            });

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

            if (projected.magnitude < _deadzone)
            {
                projected = Vector2.zero;
            }

            return projected;
        }

        private Vector2 ConvertOrientationToAimClosed(Quaternion fanOrientation)
        {
            // Roll uses up axis
            Vector3 dir = fanOrientation * Vector3.up;

            // Closed fan needs to go backwards
            /*if (dir.z > 0)
            {
                return Vector2.zero;
            }*/

            // Project onto XY plane to get horizontal aim direction
            var projected = new Vector2(dir.x, dir.y);
            projected.Normalize();

            // Closed mode doesn't use vertical input
            projected.y = 0;

            projected *= _closeSensitivity;
            projected.x = Mathf.Clamp(projected.x, -1f, 1f);
            projected.y = Mathf.Clamp(projected.y, -1f, 1f);

            if (projected.magnitude < _deadzone)
            {
                projected = Vector2.zero;
            }

            return projected;
        }

        public Quaternion SetDefaultToCurrent()
        {
            ZeroedRawOrientation = _lastRawOrientation;
            return ZeroedRawOrientation;
        }
    }
}