using Input.DataTypes;
using Input.ValueSOs;
using UnityEngine;
using UnityEngine.Events;
using ValueSO;

namespace Input
{
    /// <summary>
    ///     Interface for input
    /// </summary>
    public class GameplayInputService : MonoBehaviour, IValueSOObserver
    {
        [Header("ValueSO (Read/Write)")]

        [SerializeField]
        private GameplayInputTypeValueSO _gameplayInputTypeValueSO;

        [Header("Input Providers")]

        [SerializeField]
        private InputProvider _customHardwareInputProvider;

        [SerializeField]
        private InputProvider _conventionalInputProvider;

        [Header("Events")]

        public UnityEvent<FanState> OnFanStateChange;

        public UnityEvent<AimInput> OnAimInputChange;

        public UnityEvent OnUpdraftInput;
        public UnityEvent OnGustInput;
        public UnityEvent OnSliceInput;
        public UnityEvent OnFanSelfInput;

        public FanState CurrentFanState => _currentFanState;

        private FanState _currentFanState = FanState.Closed;
        private GameplayInputType currentGameplayInputType = GameplayInputType.None;

        private InputProvider _currentInputProvider;

        public void Initialize()
        {
            _gameplayInputTypeValueSO.AddListener(this, OnGameplayInputTypeChanged, true);
        }

        private void OnGameplayInputTypeChanged(GameplayInputType type)
        {
            SwitchInputType(type);
        }

        private void OnDestroy()
        {
            if (_currentInputProvider)
            {
                UnsubscribeInputProvider(_currentInputProvider);
            }
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

            _gameplayInputTypeValueSO.SetValue(newGameplayInputType, this);
        }

        private void SwitchInputProvidersHandlers(InputProvider newInputProvider)
        {
            if (_currentInputProvider != null)
            {
                UnsubscribeInputProvider(_currentInputProvider);
            }

            _currentInputProvider = newInputProvider;

            SubscribeInputProvider(newInputProvider);
        }

        private void SubscribeInputProvider(InputProvider inputProvider)
        {
            inputProvider.AimInputChanged += HandleAimInputChanged;
            inputProvider.DesiredFanStateChanged += HandleDesiredFanStateChanged;
            inputProvider.ToggleFanState += HandleToggleFanState;
            inputProvider.UpdraftInput += HandleUpdraftInput;
            inputProvider.SliceInput += HandleSliceInput;
            inputProvider.GustInput += HandleGustInput;
            inputProvider.FanSelfInput += HandleSelfFanInput;
        }

        private void UnsubscribeInputProvider(InputProvider inputProvider)
        {
            inputProvider.AimInputChanged -= HandleAimInputChanged;
            inputProvider.DesiredFanStateChanged -= HandleDesiredFanStateChanged;
            inputProvider.ToggleFanState -= HandleToggleFanState;
            inputProvider.UpdraftInput -= HandleUpdraftInput;
            inputProvider.SliceInput -= HandleSliceInput;
            inputProvider.GustInput -= HandleGustInput;
            inputProvider.FanSelfInput -= HandleSelfFanInput;
        }

        private void HandleSelfFanInput()
        {
            OnFanSelfInput?.Invoke();
        }

        private void HandleGustInput()
        {
            OnGustInput?.Invoke();
        }

        private void HandleSliceInput()
        {
            OnSliceInput?.Invoke();
        }

        private void HandleUpdraftInput()
        {
            OnUpdraftInput?.Invoke();
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

        public void SetZeroedOrientationToCurrent()
        {
            _conventionalInputProvider.SetDefaultToCurrent();
            _customHardwareInputProvider.SetDefaultToCurrent();
        }

        public void WriteFanOn(bool fanOn)
        {
        }
    }
}