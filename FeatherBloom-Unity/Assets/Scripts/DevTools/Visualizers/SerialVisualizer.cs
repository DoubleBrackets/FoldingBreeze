using Framework;
using Input;
using Input.DataTypes;
using UnityEngine;

namespace DevTools.Visualizers
{
    public class SerialVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private GameplayInputService _inputService;

        private void Start()
        {
            _inputService = ServiceLocator.GetService<GameplayInputService>();
            _inputService.OnAimInputChange.AddListener(HandleAimInput);
        }

        private void OnDestroy()
        {
            _inputService.OnAimInputChange.RemoveListener(HandleAimInput);
        }

        private void HandleAimInput(AimInput input)
        {
            _target.localRotation = input.ProcessedFanOrientation;
        }
    }
}