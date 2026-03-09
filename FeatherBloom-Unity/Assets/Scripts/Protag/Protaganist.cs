using System;
using DevTools;
using Framework.Timescaling;
using Input;
using Input.DataTypes;
using Input.Processor;
using Protag.Movement;
using StateMachine;
using UI;
using UnityEngine;
using ValueSO.Core;

namespace Protag
{
    public class Protaganist : MonoBehaviour
    {
        [SerializeField]
        private ProtagStateDecisionTree _protagStateDecisionTree;

        [SerializeField]
        private StateManager _protagStateMachine;

        [SerializeField]
        private Transform _protagBody;

        [SerializeField]
        private Rigidbody _protagRigidbody;

        [Header("Depends")]

        [SerializeField]
        private GroundChecker _protagGroundChecker;

        [Header("Config")]

        [SerializeField]
        private CustomFanInputConfigSO _fanInputConfig;

        [SerializeField]
        private float _resetTimeAfterDeath;

        [Header("Effects")]

        [SerializeField]
        private TimeScaleEntryConfig _untrackedInputTimeScale;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isFanOpen;

        [SerializeField]
        private BoolValueSO _isGrounded;

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }
        public event Action OnLoadResultScreen;

        private bool IsFanOpen
        {
            get => _isFanOpen.Value;
            set => _isFanOpen.SetValue(value);
        }

        private GameplayInputService _inputService;
        private TimeScaleService _timeScaleService;

        private CustomControllerInputProcessor _inputProcessor;
        private ProtagState CurrentState => _protagStateMachine.CurrentState as ProtagState;

        private float _resetTimer;

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
            _inputService.OnFanSelfInput.AddListener(HandleFanSelf);

            HandleFanStateChange(_inputService.CurrentFanState);
        }

        private void OnDestroy()
        {
            _inputService.OnAimInputChange.RemoveListener(HandleAimInputChange);
            _inputService.OnFanStateChange.RemoveListener(HandleFanStateChange);
            _inputService.OnUpdraftInput.RemoveListener(HandleTryUpdraft);
            _inputService.OnFanSelfInput.RemoveListener(HandleFanSelf);

            _protagStateMachine.Deinitialize();
        }

        private void Update()
        {
            ProtagState newState = _protagStateDecisionTree.EvaluateNewState(
                CurrentState,
                _protagGroundChecker.LastGroundedInfo,
                IsFanOpen);
            _protagStateMachine.SwitchState(newState);

            _protagStateMachine.UpdateStateMachine();

            if (_resetTimer > 0)
            {
                _resetTimer -= Time.deltaTime;
                if (_resetTimer <= 0)
                {
                    OnLoadResultScreen?.Invoke();
                }
            }
        }

        private void FixedUpdate()
        {
            _protagGroundChecker.CheckGrounded();
            _isGrounded.SetValue(_protagGroundChecker.LastGroundedInfo.IsGrounded);

            ProtagState newState = _protagStateDecisionTree.EvaluateNewState(
                CurrentState,
                _protagGroundChecker.LastGroundedInfo,
                IsFanOpen);
            _protagStateMachine.SwitchState(newState);

            _protagStateMachine.FixedUpdateStateMachine();
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
            ProtagState newState = _protagStateDecisionTree.EvaluateNewState(
                CurrentState,
                _protagGroundChecker.LastGroundedInfo,
                IsFanOpen,
                tryHealing: true);
            _protagStateMachine.SwitchState(newState);
        }

        private void HandleTryUpdraft()
        {
            ProtagState newState = _protagStateDecisionTree.EvaluateNewState(
                CurrentState,
                _protagGroundChecker.LastGroundedInfo,
                IsFanOpen,
                true);
            _protagStateMachine.SwitchState(newState);
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
            if (_protagStateDecisionTree.IsInOneshotState(CurrentState) ||
                processResult.CurrentState == InputProcessorState.Tracking)
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
            ProtagState newState = _protagStateDecisionTree.EvaluateNewState(
                CurrentState,
                _protagGroundChecker.LastGroundedInfo,
                IsFanOpen,
                shouldDie: true);
            _protagStateMachine.SwitchState(newState);

            _resetTimer = _resetTimeAfterDeath;
        }
    }
}