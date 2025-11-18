using UnityEngine;
using UnityEngine.Events;

namespace Input
{
    /// <summary>
    ///     Interface for input
    /// </summary>
    public class GameplayInputService : MonoBehaviour
    {
        public enum FanState
        {
            Closed,
            Open
        }

        public enum GameplayInputType
        {
            None,
            Conventional,
            CustomHardware
        }

        public struct AimInput
        {
            public Vector2 FinalAimInput;
            public Quaternion ProcessedFanOrientation;
            public Quaternion RawFanOrientation;
        }

        [Header("Input Providers")]

        [SerializeField]
        private InputProvider _customHardwareInputProvider;

        [SerializeField]
        private InputProvider _conventionalInputProvider;

        [Header("Events")]

        public UnityEvent<FanState> OnFanStateChange;

        public UnityEvent<AimInput> OnAimInputChange;

        public static GameplayInputService Instance { get; private set; }

        private FanState _currentFanState = FanState.Closed;
        private GameplayInputType currentGameplayInputType = GameplayInputType.None;
        private InputProvider _currentInputProvider;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            SwitchInputType(GameplayInputType.Conventional);
        }

        public void SwitchInputType(GameplayInputType newGameplayInputType)
        {
            if (newGameplayInputType == currentGameplayInputType)
            {
                return;
            }

            Debug.Log($"Switching input type to {newGameplayInputType}");

            if (newGameplayInputType == GameplayInputType.Conventional)
            {
                currentGameplayInputType = GameplayInputType.Conventional;
                SwitchInputProvidersHandlers(_conventionalInputProvider);
            }
            else if (newGameplayInputType == GameplayInputType.CustomHardware)
            {
                currentGameplayInputType = GameplayInputType.CustomHardware;
                SwitchInputProvidersHandlers(_customHardwareInputProvider);
            }
        }

        private void SwitchInputProvidersHandlers(InputProvider newInputProvider)
        {
            if (_currentInputProvider != null)
            {
                _currentInputProvider.AimInputChanged -= HandleAimInputChanged;
                _currentInputProvider.DesiredFanStateChanged -= HandleDesiredFanStateChanged;
                newInputProvider.ToggleFanState -= HandleToggleFanState;
            }

            _currentInputProvider = newInputProvider;

            newInputProvider.AimInputChanged += HandleAimInputChanged;
            newInputProvider.DesiredFanStateChanged += HandleDesiredFanStateChanged;
            newInputProvider.ToggleFanState += HandleToggleFanState;
        }

        private void HandleToggleFanState()
        {
            HandleDesiredFanStateChanged(_currentFanState == FanState.Open ? FanState.Closed : FanState.Open);
        }

        private void HandleDesiredFanStateChanged(FanState desiredState)
        {
            if (desiredState == FanState.Open && _currentFanState == FanState.Closed)
            {
                _currentFanState = FanState.Open;
                OnFanStateChange?.Invoke(_currentFanState);
            }
            else if (desiredState == FanState.Closed && _currentFanState == FanState.Open)
            {
                _currentFanState = FanState.Closed;
                OnFanStateChange?.Invoke(_currentFanState);
            }
        }

        private void HandleAimInputChanged(AimInput aimInput)
        {
            OnAimInputChange?.Invoke(aimInput);
        }
    }
}