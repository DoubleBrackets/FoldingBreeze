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

        public static Protaganist Instance { get; private set; }

        public Vector3 Position => _protagBody.position;
        public Vector2 AimInput { get; private set; }

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

        private void HandleAimInputChange(GameplayInputService.AimInput aimInput)
        {
            AimInput = aimInput.NormalizedAimInput;
        }

        private void HandleFanStateChange(GameplayInputService.FanState state)
        {
        }

        public void SetPosition(Vector3 position)
        {
            _protagBody.position = position;
        }
    }
}