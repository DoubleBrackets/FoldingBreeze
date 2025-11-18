using Input;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class CursorInputUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _horizonLine;

        [SerializeField]
        private Slider _verticalIndicator;

        [SerializeField]
        private float _horizonTiltRange;

        private void Start()
        {
            GameplayInputService.Instance.OnFanStateChange.AddListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleAimInputChange);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(HandleFanStateChange);
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleAimInputChange);
        }

        private void HandleAimInputChange(GameplayInputService.AimInput aim)
        {
            _horizonLine.rotation = Quaternion.Euler(0, 0, -aim.FinalAimInput.x * _horizonTiltRange);
            _verticalIndicator.value = aim.FinalAimInput.y.RemapOnesTo01();
        }

        private void HandleFanStateChange(GameplayInputService.FanState state)
        {
        }
    }
}