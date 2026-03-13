using Framework;
using Input;
using Input.DataTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevTools.Visualizers
{
    /// <summary>
    ///     Debug tool for visualizing input in UI
    /// </summary>
    public class InputVisualizer : MonoBehaviour
    {
        [Header("Aim Visualizer")]

        [SerializeField]
        private Image aimCursor;

        [SerializeField]
        private TMP_Text aimText;

        [SerializeField]
        private float aimAreaSize;

        [Header("Fan State")]

        [SerializeField]
        private GameObject fanOpenIndicator;

        [SerializeField]
        private GameObject fanCloseIndicator;

        private Quaternion _lastOrientation;

        private GameplayInputService _gameplayInputService;

        private void Start()
        {
            _gameplayInputService = ServiceLocator.GetService<GameplayInputService>();
            _gameplayInputService.OnFanStateChange.AddListener(OnFanStateChanged);
            _gameplayInputService.OnAimInputChange.AddListener(OnAimInputChanged);
        }

        private void OnDestroy()
        {
            _gameplayInputService.OnFanStateChange.RemoveListener(OnFanStateChanged);
            _gameplayInputService.OnAimInputChange.RemoveListener(OnAimInputChanged);
        }

        private void OnDrawGizmos()
        {
            var axisLength = 2f;
            // Draw axis
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, Vector3.up * axisLength);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.right * axisLength);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * axisLength);

            // Draw forward orientation
            Vector3 forward = _lastOrientation * Vector3.forward;
            DrawAxisWithCube(forward, Color.cyan);

            // Draw up orientation
            Vector3 up = _lastOrientation * Vector3.up;
            DrawAxisWithCube(up, new Color(0.49f, 1f, 0.65f));

            // Draw right orientation
            Vector3 right = _lastOrientation * Vector3.right;
            DrawAxisWithCube(right, new Color(1f, 0.49f, 0));
        }

        private void DrawAxisWithCube(Vector3 direction, Color color)
        {
            var axisLength = 2f;
            Gizmos.color = color;
            Gizmos.DrawLine(Vector3.zero, direction * axisLength);
            Gizmos.DrawSphere(direction * axisLength, 0.2f);
        }

        private void OnFanStateChanged(FanState state)
        {
            fanCloseIndicator.SetActive(state == FanState.Closed);
            fanOpenIndicator.SetActive(state == FanState.Open);
        }

        private void OnAimInputChanged(AimInput aimInput)
        {
            Vector2 normalizedAimInput = aimInput.FinalAimInput;
            aimCursor.rectTransform.anchoredPosition =
                new Vector2(normalizedAimInput.x, normalizedAimInput.y) * aimAreaSize;

            aimText.text = $"({normalizedAimInput.x:F2}, {normalizedAimInput.y:F2})";

            _lastOrientation = aimInput.ProcessedFanOrientation;
        }
    }
}