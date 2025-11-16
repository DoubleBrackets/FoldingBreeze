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

        private void Start()
        {
            GameplayInputService.Instance.OnFanStateChange.AddListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleFanAimInputChange);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleFanAimInputChange);
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