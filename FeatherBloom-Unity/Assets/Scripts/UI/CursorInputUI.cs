using Framework;
using Input;
using Input.DataTypes;
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

        private GameplayInputService _inputService;

        private void Start()
        {
            _inputService = ServiceLocator.GetService<GameplayInputService>();
            _inputService.OnFanStateChange.AddListener(HandleFanStateChange);
            _inputService.OnAimInputChange.AddListener(HandleAimInputChange);
        }

        private void OnDestroy()
        {
            _inputService.OnFanStateChange.RemoveListener(HandleFanStateChange);
            _inputService.OnAimInputChange.RemoveListener(HandleAimInputChange);
        }

        private void HandleAimInputChange(AimInput aim)
        {
            _horizonLine.rotation = Quaternion.Euler(0, 0, -aim.FinalAimInput.x * _horizonTiltRange);
            _verticalIndicator.value = aim.FinalAimInput.y.RemapOnesTo01();
        }

        private void HandleFanStateChange(FanState state)
        {
        }
    }
}