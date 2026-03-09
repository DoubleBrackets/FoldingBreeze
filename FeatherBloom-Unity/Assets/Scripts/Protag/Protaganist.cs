using System;
using DevTools;
using Events;
using Framework.Timescaling;
using Input;
using Input.DataTypes;
using Input.Processor;
using Protag.GestureHandlers;
using StateMachine;
using UI;
using UnityEngine;
using ValueSO.Core;

namespace Protag
{
    public class Protaganist : MonoBehaviour
    {
        [SerializeField]
        private StateManager _protagStateMachine;

        [SerializeField]
        private Transform _protagBody;

        [SerializeField]
        private Rigidbody _protagRigidbody;

        [SerializeField]
        private CustomFanInputConfigSO _fanInputConfig;

        [Header("Effects")]

        [SerializeField]
        private TimeScaleEntryConfig _untrackedInputTimeScale;

        [Header("Event Out")]

        [SerializeField]
        private VoidEvent _onDeath;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isFanOpen;

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }

        public bool IsFanOpen { get; private set; }

        public event Action OnDeath;

        private GameplayInputService _inputService;
        private TimeScaleService _timeScaleService;

        private CustomControllerInputProcessor _inputProcessor;

        private IUpdraftHandler _updraftHandler;
        private IGustHandler _gustHandler;
        private IBreezeHandler _fanSelfHandler;

        /// <summary>
        ///     After making a gesture, prevent time slowdown from untracked input for a short time
        /// </summary>
        /// <returns></returns>
        private float _postGestureBlockTimescaleTimer;

        public void RegisterUpdraftHandler(IUpdraftHandler handler)
        {
            _updraftHandler = handler;
        }

        public void RegisterGustHandler(IGustHandler handler)
        {
            _gustHandler = handler;
        }

        public void RegisterFanSelfHandler(IBreezeHandler handler)
        {
            _fanSelfHandler = handler;
        }

        private void Awake()
        {
            _protagStateMachine.Initialize();
            _inputProcessor = new CustomControllerInputProcessor(_fanInputConfig);

            _isFanOpen.SetValue(IsFanOpen);
        }

        public void Initialize(TimeScaleService timeScaleService, GameplayInputService inputService)
        {
            _inputService = inputService;
            _timeScaleService = timeScaleService;
        }

        private void Start()
        {
            _inputService.OnAimInputChange.AddListener(HandleAimInputChange);
            _inputService.OnFanStateChange.AddListener(HandleFanStateChange);
            _inputService.OnUpdraftInput.AddListener(HandleTryUpdraft);
            _inputService.OnGustInput.AddListener(HandleTryGust);
            _inputService.OnFanSelfInput.AddListener(HandleFanSelf);

            HandleFanStateChange(_inputService.CurrentFanState);
        }

        private void Update()
        {
            _postGestureBlockTimescaleTimer -= Time.unscaledDeltaTime;
        }

        private void OnDestroy()
        {
            _inputService.OnAimInputChange.RemoveListener(HandleAimInputChange);
            _inputService.OnFanStateChange.RemoveListener(HandleFanStateChange);
            _inputService.OnUpdraftInput.RemoveListener(HandleTryUpdraft);
            _inputService.OnGustInput.RemoveListener(HandleTryGust);
            _inputService.OnFanSelfInput.RemoveListener(HandleFanSelf);

            _protagStateMachine.Deinitialize();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                LabelUtils.Label(_protagBody.position, $"{_protagStateMachine.CurrentState.name}");
            }
        }

        private void HandleFanSelf()
        {
            GestureHandleResult result = _fanSelfHandler.HandleBreeze();
            if (result.DidSucceed)
            {
                _postGestureBlockTimescaleTimer = result.TimeslowdownBlockDuration;
            }
        }

        private void HandleTryGust()
        {
            GestureHandleResult result = _gustHandler.HandleGust();
            if (result.DidSucceed)
            {
                _postGestureBlockTimescaleTimer = result.TimeslowdownBlockDuration;
            }
        }

        private void HandleTryUpdraft()
        {
            GestureHandleResult result = _updraftHandler.HandleUpdraft();
            if (result.DidSucceed)
            {
                _postGestureBlockTimescaleTimer = result.TimeslowdownBlockDuration;
            }
        }

        private void HandleAimInputChange(AimInput aimInput)
        {
            ProcessResult processResult = _inputProcessor.ProcessInput(_inputService.CurrentInputType, aimInput);
            AimInput = processResult.ProcessedAimInput;

            // If untracked, slow down to give the player time to move between gestures and aiming
            if (processResult.CurrentState == InputProcessorState.Untracked)
            {
                _timeScaleService.NewTimeScaling(_untrackedInputTimeScale);
            }

            // Except for right after a gesture, since we want gestures to play at normal speed
            if (_postGestureBlockTimescaleTimer > 0f || processResult.CurrentState == InputProcessorState.Tracking)
            {
                _timeScaleService.RemoveTimeScale(_untrackedInputTimeScale.Identifier);
            }
        }

        private void HandleFanStateChange(FanState state)
        {
            IsFanOpen = state == FanState.Open;
            _isFanOpen.SetValue(IsFanOpen);
        }

        public void SetPositionAndDirection(Vector3 position, Vector3 direction)
        {
            _protagBody.position = position;
            _protagRigidbody.linearVelocity = direction;
        }

        public void Kill()
        {
            _onDeath?.Raise();
            OnDeath?.Invoke();
        }
    }
}