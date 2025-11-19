using Input;
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
        public UnityEvent<GameplayInputService.AimInput> OnAimInputChange;

        [SerializeField]
        public UnityEvent OnUpdraft;

        private void Start()
        {
            GameplayInputService.Instance.OnFanStateChange.AddListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleFanAimInputChange);
            GameplayInputService.Instance.OnUpdraftInput.AddListener(HandleUpdraftInput);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleFanAimInputChange);
            GameplayInputService.Instance.OnUpdraftInput.RemoveListener(HandleUpdraftInput);
        }

        private void HandleUpdraftInput()
        {
            OnUpdraft?.Invoke();
        }

        private void HandleFanStateChange(GameplayInputService.FanState state)
        {
            if (state == GameplayInputService.FanState.Open)
            {
                OnFanOpen?.Invoke();
            }
            else if (state == GameplayInputService.FanState.Closed)
            {
                OnFanClose?.Invoke();
            }
        }

        private void HandleFanAimInputChange(GameplayInputService.AimInput aimInput)
        {
            OnAimInputChange?.Invoke(aimInput);
        }
    }
}