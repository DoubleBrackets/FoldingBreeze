using DevTools;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using ValueSO.Core;

namespace Protag.LevelGen
{
    /// <summary>
    ///     Represents a section of a stage in the level generation system.
    /// </summary>
    public class MapStage : MonoBehaviour
    {
        [Header("Layout Info")]

        [SerializeField]
        private Transform _endingHeight;

        [SerializeField]
        private float _stageAngularWidthInDegrees;

        [SerializeField]
        private float _killZoneVerticalPos;

        [SerializeField]
        private FloatValueSO _pillarRadius;

        [Header("Depends")]

        [SerializeField]
        private GameObject _terrainContainer;

        [SerializeField]
        [Layer]
        private string _terrainContainerLayer;

        [Header("Animation")]

        [SerializeField]
        private float _riseLerpFactor;

        [SerializeField]
        private float _riseLinearSpeed;

        // Used to animate the stage rising from the ground
        private Vector3 _targetPos;
        public float KillZoneVerticalPos => _killZoneVerticalPos;
        public float StageAngularWidth => _stageAngularWidthInDegrees;
        public float Height => _endingHeight.localPosition.y;

        public float EndingAngularPos => _placementAnglePos + _stageAngularWidthInDegrees;

        private float _placementAnglePos;

        private void OnDrawGizmos()
        {
            float innerRadius = _pillarRadius.Value;
            float outerRadius = _pillarRadius.Value + 50;
            float startY = transform.position.y;
            float height = Height;

            Vector3 startCenter = Vector3.up * startY;

            Handles.zTest = CompareFunction.LessEqual;
            HandleUtils.DrawArc(startCenter, _placementAnglePos, -_stageAngularWidthInDegrees, outerRadius,
                HandleUtils.TransparentGreen);
            HandleUtils.DrawArc(startCenter + Vector3.up * height, _placementAnglePos, -_stageAngularWidthInDegrees,
                outerRadius,
                HandleUtils.TransparentWhite);

            HandleUtils.DrawWireRectSlice(_placementAnglePos, innerRadius, outerRadius, startY, height,
                HandleUtils.TransparentGreen);
            HandleUtils.DrawWireRectSlice(EndingAngularPos, innerRadius, outerRadius, startY, height,
                HandleUtils.TransparentWhite);

            // Kill zone
            HandleUtils.DrawArc(startCenter + Vector3.up * _killZoneVerticalPos,
                _placementAnglePos,
                -_stageAngularWidthInDegrees,
                outerRadius, HandleUtils.TransparentRed);
        }

        private void Update()
        {
            float t = 1 - Mathf.Pow(0.01f, Time.deltaTime * _riseLerpFactor);
            transform.position = Vector3.Lerp(transform.position, _targetPos, t);
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _riseLinearSpeed * Time.deltaTime);
        }

        public void Initialize(float startPositionY, float placementAngle, bool riseAnimation = false)
        {
            // Reorient the stage to match the start position
            var offset = new Vector3(0f, startPositionY, 0f);
            transform.position += offset;
            _targetPos = transform.position;

            if (riseAnimation)
            {
                transform.position += Vector3.down * 100f;
            }

            // Rotate to match forward direction
            // Negate, since from top-down view we need to flip the angle
            Quaternion targetRotation = Quaternion.Euler(0f, -placementAngle, 0f);
            transform.rotation = targetRotation;

            _placementAnglePos = placementAngle;
        }

        [Button("Fix Layers")]
        public void SetLayer()
        {
            // Iterate over all children in terrain
            foreach (Transform child in _terrainContainer.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer(_terrainContainerLayer);
            }
        }
    }
}