using Framework;
using Input;
using Input.DataTypes;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    /// <summary>
    ///     Exposes input service as Unity Events
    /// </summary>
    public class InputMono : MonoBehaviour
    {
        [Header("Events")]

        [SerializeField]
        public UnityEvent OnFanOpen;

        [SerializeField]
        public UnityEvent OnFanClose;

        [SerializeField]
        public UnityEvent<AimInput> OnAimInputChange;

        [SerializeField]
        public UnityEvent OnUpdraft;

        [SerializeField]
        public UnityEvent OnFanSelf;

        private GameplayInputService _inputService;

        private void Start()
        {
            _inputService = ServiceLocator.GetService<GameplayInputService>();
            _inputService.OnFanStateChange.AddListener(HandleFanStateChange);
            _inputService.OnAimInputChange.AddListener(HandleFanAimInputChange);
            _inputService.OnUpdraftInput.AddListener(HandleUpdraftInput);
            _inputService.OnFanSelfInput.AddListener(HandleFanSelfInput);
        }

        private void OnDestroy()
        {
            _inputService.OnFanStateChange.RemoveListener(HandleFanStateChange);
            _inputService.OnAimInputChange.RemoveListener(HandleFanAimInputChange);
            _inputService.OnUpdraftInput.RemoveListener(HandleUpdraftInput);
            _inputService.OnFanSelfInput.RemoveListener(HandleFanSelfInput);
        }

        private void HandleFanSelfInput()
        {
            OnFanSelf?.Invoke();
        }

        private void HandleUpdraftInput()
        {
            OnUpdraft?.Invoke();
        }

        private void HandleFanStateChange(FanState state)
        {
            if (state == FanState.Open)
            {
                OnFanOpen?.Invoke();
            }
            else if (state == FanState.Closed)
            {
                OnFanClose?.Invoke();
            }
        }

        private void HandleFanAimInputChange(AimInput aimInput)
        {
            OnAimInputChange?.Invoke(aimInput);
        }
    }
}