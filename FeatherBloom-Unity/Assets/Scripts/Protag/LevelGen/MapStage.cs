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

        [Header("Events")]

        [SerializeField]
        public UnityEvent OnStageSectionEntered;

        [SerializeField]
        public UnityEvent OnStageSectionExited;

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

        public void Initialize(Vector3 startPosition)
        {
            // Reorient the stage to match the start position
            Vector3 offset = startPosition - _startPoint.position;
            transform.position += offset;
        }
    }
}