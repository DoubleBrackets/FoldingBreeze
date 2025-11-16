using Input;
using UnityEngine;

namespace DebugTools.Visualizers
{
    public class SerialVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private void Start()
        {
            GameplayInputService.Instance.OnAimInputChange.AddListener(HandleAimInput);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(HandleAimInput);
        }

        private void HandleAimInput(GameplayInputService.AimInput input)
        {
            _target.rotation = input.ProcessedFanOrientation;
        }
    }
}