using System;
using DevTools;
using Events;
using Framework;
using Input;
using Input.DataTypes;
using StateMachine;
using UnityEngine;

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

        [Header("Event Out")]

        [SerializeField]
        private VoidEvent _onFanOpen;

        [SerializeField]
        private VoidEvent _onFanClose;

        [SerializeField]
        private VoidEvent _onDeath;

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }

        public bool IsFanOpen { get; private set; }
        public event Action OnFanOpen;

        public event Action OnTryUpdraft;
        public event Action OnTryGust;
        public event Action OnTryFanSelf;

        public event Action OnDeath;

        private GameplayInputService _inputService;

        private void Awake()
        {
            _protagStateMachine.Initialize();
        }

        private void Start()
        {
            _inputService = ServiceLocator.GetService<GameplayInputService>();
            _inputService.OnAimInputChange.AddListener(HandleAimInputChange);
            _inputService.OnFanStateChange.AddListener(HandleFanStateChange);
            _inputService.OnUpdraftInput.AddListener(HandleTryUpdraft);
            _inputService.OnGustInput.AddListener(HandleTryGust);
            _inputService.OnFanSelfInput.AddListener(HandleFanSelf);

            HandleFanStateChange(_inputService.CurrentFanState);
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
            OnTryFanSelf?.Invoke();
        }

        private void HandleTryGust()
        {
            OnTryGust?.Invoke();
        }

        private void HandleTryUpdraft()
        {
            OnTryUpdraft?.Invoke();
        }

        private void HandleAimInputChange(AimInput aimInput)
        {
            AimInput = aimInput.FinalAimInput;
        }

        private void HandleFanStateChange(FanState state)
        {
            IsFanOpen = state == FanState.Open;
            if (IsFanOpen)
            {
                OnFanOpen?.Invoke();
                _onFanOpen?.Raise();
            }
            else
            {
                _onFanClose?.Raise();
            }
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