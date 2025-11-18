using Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugTools.Visualizers
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

        private void Start()
        {
            GameplayInputService.Instance.OnFanStateChange.AddListener(OnFanStateChanged);
            GameplayInputService.Instance.OnAimInputChange.AddListener(OnAimInputChanged);
        }

        private void OnDestroy()
        {
            GameplayInputService.Instance.OnFanStateChange.RemoveListener(OnFanStateChanged);
            GameplayInputService.Instance.OnAimInputChange.RemoveListener(OnAimInputChanged);
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

        private void OnFanStateChanged(GameplayInputService.FanState state)
        {
            fanCloseIndicator.SetActive(state == GameplayInputService.FanState.Closed);
            fanOpenIndicator.SetActive(state == GameplayInputService.FanState.Open);
        }

        private void OnAimInputChanged(GameplayInputService.AimInput aimInput)
        {
            Vector2 normalizedAimInput = aimInput.FinalAimInput;
            aimCursor.rectTransform.anchoredPosition =
                new Vector2(normalizedAimInput.x, normalizedAimInput.y) * aimAreaSize;

            aimText.text = $"({normalizedAimInput.x:F2}, {normalizedAimInput.y:F2})";

            _lastOrientation = aimInput.ProcessedFanOrientation;
        }
    }
}