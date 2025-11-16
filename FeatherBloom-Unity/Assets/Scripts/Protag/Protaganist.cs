using DebugTools;
using Input;
using StateMachine;
using UnityEngine;
using UnityEngine.Events;

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

        [Header("Events")]

        [SerializeField]
        private UnityEvent _onFanOpen;

        [SerializeField]
        private UnityEvent _onFanClose;

        [SerializeField]
        private UnityEvent _onDeath;

        public static Protaganist Instance { get; private set; }

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }

        public bool IsFanOpen { get; private set; }

        private void Awake()
        {
            Instance = this;
            _protagStateMachine.Initialize();
        }

        private void Start()
        {
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleAimInputChange);
            GameplayInputService.Instance.OnFanStateChange.AddListener(HandleFanStateChange);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleAimInputChange);
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(HandleFanStateChange);

            _protagStateMachine.Deinitialize();
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                LabelUtils.Label(_protagBody.position, $"{_protagStateMachine.CurrentState.name}");
            }
        }

        private void HandleAimInputChange(GameplayInputService.AimInput aimInput)
        {
            AimInput = aimInput.NormalizedAimInput;
        }

        private void HandleFanStateChange(GameplayInputService.FanState state)
        {
            IsFanOpen = state == GameplayInputService.FanState.Open;
            if (IsFanOpen)
            {
                _onFanOpen?.Invoke();
            }
            else
            {
                _onFanClose?.Invoke();
            }
        }

        public void SetPositionAndDirection(Vector3 position, Vector3 direction)
        {
            _protagBody.position = position;
            _protagRigidbody.linearVelocity = direction;
        }

        public void Kill()
        {
            _onDeath?.Invoke();
        }
    }
}