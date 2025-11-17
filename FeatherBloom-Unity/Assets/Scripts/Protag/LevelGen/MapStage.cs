using DebugTools;
using UnityEngine;
using UnityEngine.Events;

namespace Protag.LevelGen
{
    /// <summary>
    ///     Represents a section of a stage in the level generation system.
    /// </summary>
    public class MapStage : MonoBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Transform _startPoint;

        [SerializeField]
        private Transform _endPoint;

        [SerializeField]
        private StageTrigger _stageTrigger;

        [SerializeField]
        private float _killZoneHeight;

        [Header("Config")]

        [SerializeField]
        private float _riseLerpFactor;

        [SerializeField]
        private float _riseLinearSpeed;

        [Header("Events")]

        [SerializeField]
        public UnityEvent OnStageSectionEntered;

        [SerializeField]
        public UnityEvent OnStageSectionExited;

        private bool _stageEnabled;

        // Used to animate the stage rising from the ground
        private Vector3 _targetPos;

        private void Awake()
        {
            _stageTrigger.OnProtagEnterSection.AddListener(HandleStageSectionEntered);
            _stageTrigger.OnProtagExitSection.AddListener(HandleStageSectionExited);
        }

        private void OnDestroy()
        {
            _stageTrigger.OnProtagEnterSection.RemoveListener(HandleStageSectionEntered);
            _stageTrigger.OnProtagExitSection.RemoveListener(HandleStageSectionExited);
        }

        private void OnDrawGizmos()
        {
            LabelUtils.Label(_startPoint.position, "STAGE START");
            LabelUtils.Label(_endPoint.position, "STAGE END");

            Vector3 endPos = GetEndPosition();
            Vector3 startPos = GetStartPosition();
            Vector3 endDir = GetEndForward();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawLine(endPos, endPos + endDir * 5f);
            Gizmos.DrawSphere(endPos + endDir * 5f, 1f);
            Gizmos.DrawLine(startPos, startPos + Vector3.forward * 5f);
            Gizmos.DrawSphere(startPos - Vector3.forward * 5f, 0.5f);

            // Kill zone
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(transform.position + Vector3.down * _killZoneHeight, new Vector3(100, 1, 100));
        }

        private void Update()
        {
            if (_stageEnabled)
            {
                Vector3 playerPos = Protaganist.Instance.Position;
                if (playerPos.y < transform.position.y - _killZoneHeight)
                {
                    Protaganist.Instance.Kill();
                }
            }

            float t = 1 - Mathf.Pow(0.01f, Time.deltaTime * _riseLerpFactor);
            transform.position = Vector3.Lerp(transform.position, _targetPos, t);
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _riseLinearSpeed * Time.deltaTime);
        }

        public void SetStageEnabled(bool enable)
        {
            _stageEnabled = enable;
        }

        private void HandleStageSectionEntered()
        {
            OnStageSectionEntered?.Invoke();
        }

        private void HandleStageSectionExited()
        {
            OnStageSectionExited?.Invoke();
        }

        public Vector3 GetStartPosition()
        {
            return _startPoint.position;
        }

        public Vector3 GetEndPosition()
        {
            return _endPoint.position;
        }

        public Vector3 GetEndForward()
        {
            Vector3 forward = _endPoint.forward;
            forward.y = 0;
            return forward.normalized;
        }

        public void Initialize(Vector3 startPosition, Vector3 forward, bool riseAnimation = false)
        {
            // Reorient the stage to match the start position
            Vector3 offset = startPosition - _startPoint.position;
            transform.position += offset;
            _targetPos = transform.position;

            if (riseAnimation)
            {
                transform.position += Vector3.down * 100f;
            }

            // Rotate to match forward direction
            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = targetRotation;
        }
    }
}