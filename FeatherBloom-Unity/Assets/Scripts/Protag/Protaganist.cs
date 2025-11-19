using System;
using DebugTools;
using Events;
using Input;
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

        public static Protaganist Instance { get; private set; }

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }

        public bool IsFanOpen { get; private set; }

        public event Action OnTryUpdraft;
        public event Action OnTryGust;

        private void Awake()
        {
            Instance = this;
            _protagStateMachine.Initialize();
        }

        private void Start()
        {
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleAimInputChange);
            GameplayInputService.Instance.OnFanStateChange.AddListener(HandleFanStateChange);
            GameplayInputService.Instance.OnUpdraftInput.AddListener(HandleTryUpdraft);
            GameplayInputService.Instance.OnGustInput.AddListener(HandleTryGust);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleAimInputChange);
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(HandleFanStateChange);
            GameplayInputService.Instance.OnUpdraftInput.RemoveListener(HandleTryUpdraft);
            GameplayInputService.Instance.OnGustInput.RemoveListener(HandleTryGust);

            _protagStateMachine.Deinitialize();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                LabelUtils.Label(_protagBody.position, $"{_protagStateMachine.CurrentState.name}");
            }
        }

        private void HandleTryGust()
        {
            OnTryGust?.Invoke();
        }

        private void HandleTryUpdraft()
        {
            OnTryUpdraft?.Invoke();
        }

        private void HandleAimInputChange(GameplayInputService.AimInput aimInput)
        {
            AimInput = aimInput.FinalAimInput;
        }

        private void HandleFanStateChange(GameplayInputService.FanState state)
        {
            IsFanOpen = state == GameplayInputService.FanState.Open;
            if (IsFanOpen)
            {
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
        }
    }
}