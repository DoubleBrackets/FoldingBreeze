using Framework;
using Input;
using Input.DataTypes;
using Input.Processor;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class CursorInputUI : MonoBehaviour
    {
        [SerializeField]
        private CustomFanInputConfigSO _fanInputConfig;

        [Header("Tracking UI")]

        [SerializeField]
        private CanvasGroup _trackingCanvasGroup;

        [SerializeField]
        private Transform _horizonLine;

        [SerializeField]
        private Slider _verticalIndicator;

        [SerializeField]
        private float _horizonTiltRange;

        [Header("Untracked UI")]

        [SerializeField]
        private CanvasGroup _untrackedCanvasGroup;

        [SerializeField]
        private RectTransform _cursor;

        [SerializeField]
        private RectTransform _restartTrackingIndicator;

        [SerializeField]
        private float _maxAngleCursorUIDistance;

        private GameplayInputService _inputService;
        private CustomControllerInputProcessor _inputProcessor;

        private void Start()
        {
            _inputService = ServiceLocator.GetService<GameplayInputService>();
            _inputProcessor = new CustomControllerInputProcessor(_fanInputConfig);
            _inputService.OnFanStateChange.AddListener(HandleFanStateChange);
            _inputService.OnAimInputChange.AddListener(HandleAimInputChange);

            _restartTrackingIndicator.sizeDelta = Vector2.one * _maxAngleCursorUIDistance * 2f;
        }

        private void OnDestroy()
        {
            _inputService.OnFanStateChange.RemoveListener(HandleFanStateChange);
            _inputService.OnAimInputChange.RemoveListener(HandleAimInputChange);
        }

        private void HandleAimInputChange(AimInput aim)
        {
            ProcessResult processResult = _inputProcessor.ProcessInput(_inputService.CurrentInputType, aim);

            _trackingCanvasGroup.alpha = processResult.CurrentState == InputProcessorState.Tracking ? 1 : 0;
            _untrackedCanvasGroup.alpha = processResult.CurrentState == InputProcessorState.Untracked ? 1 : 0;

            if (processResult.CurrentState == InputProcessorState.Tracking)
            {
                HandleTrackingUI(processResult);
            }
            else if (processResult.CurrentState == InputProcessorState.Untracked)
            {
                HandleUntrackedUI(processResult);
            }
        }

        private void HandleTrackingUI(ProcessResult result)
        {
            _horizonLine.rotation = Quaternion.Euler(0, 0, -result.ProcessedAimInput.x * _horizonTiltRange);
            _verticalIndicator.value = result.ProcessedAimInput.y.RemapOnesTo01();
        }

        private void HandleUntrackedUI(ProcessResult result)
        {
            // Project against XY
            Vector3 projected = Vector3.ProjectOnPlane(result.CurrentFanForward, Vector3.forward);
            var projectedAim = new Vector2(projected.x, projected.y);

            _cursor.anchoredPosition = projectedAim.normalized * result.CurrentFanAngleFromForward /
                                       _fanInputConfig.FanRestartTrackingAngle *
                                       _maxAngleCursorUIDistance;
        }

        private void HandleFanStateChange(FanState state)
        {
        }
    }
}