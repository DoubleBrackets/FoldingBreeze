using DebugTools;
using Services;
using UnityEngine;

namespace Protag.LevelGen
{
    /// <summary>
    ///     Handles simple map generation by connecting stages
    /// </summary>
    public class MapService : MonoBehaviour
    {
        [SerializeField]
        private StageRosterSO _stageRosterSO;

        public static MapService Instance { get; private set; }

        private MapStage _currentMapStage;
        private MapStage _nextMapStage;
        private MapStage _previousMapStage;

        private StageSO _lastChosenStage;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            ScoreService.Instance.ResetScore();
        }

        private void Start()
        {
            if (DebugState.DoNotLoadMapOnStart)
            {
                return;
            }

            Protaganist.Instance.SetPositionAndDirection(Vector3.zero, Vector3.forward);

            StartMap();
        }

        public StageRosterSO.RosterEntry GetRandomStageEntry()
        {
            return _stageRosterSO.GetRandomStageEntry(_lastChosenStage);
        }

        public void StartMap()
        {
            _currentMapStage = LoadInitialStage(_stageRosterSO.GetStartingStageEntry());
            _currentMapStage.SetStageEnabled(true);
            _nextMapStage = LoadStage(GetRandomStageEntry(), _currentMapStage);
            _nextMapStage.OnStageSectionEntered.AddListener(HandleOnNextStageEntered);
        }

        private MapStage LoadInitialStage(StageRosterSO.RosterEntry stageEntry)
        {
            MapStage stagePrefab = stageEntry.Prefab;
            MapStage stageInstance = Instantiate(stagePrefab, transform);
            stageInstance.Initialize(Vector3.zero, Vector3.forward);
            return stageInstance;
        }

        private void MoveToNextStage()
        {
            // Unsubscribe
            if (_nextMapStage)
            {
                _nextMapStage.OnStageSectionEntered.RemoveListener(HandleOnNextStageEntered);
            }

            // Destroy previous
            if (_previousMapStage)
            {
                Debug.Log($"Destroying previous stage: {_previousMapStage.name}");
                Destroy(_previousMapStage.gameObject);
            }

            _currentMapStage.SetStageEnabled(false);

            // Update
            _previousMapStage = _currentMapStage;
            _currentMapStage = _nextMapStage;
            Debug.Log($"Moved to new current stage: {_currentMapStage.name}");

            _currentMapStage.SetStageEnabled(true);

            _nextMapStage = LoadStage(GetRandomStageEntry(), _currentMapStage);
            Debug.Log($"Loaded new stage: {_nextMapStage.name}");

            // Subscribe
            _nextMapStage.OnStageSectionEntered.AddListener(HandleOnNextStageEntered);

            ScoreService.Instance.AddScore(1);
        }

        private void HandleOnNextStageEntered()
        {
            MoveToNextStage();
        }

        private MapStage LoadStage(StageRosterSO.RosterEntry stageEntry, MapStage previousStageInstance)
        {
            MapStage stagePrefab = stageEntry.Prefab;
            MapStage stageInstance = Instantiate(stagePrefab, transform);
            stageInstance.Initialize(previousStageInstance.GetEndPosition(), previousStageInstance.GetEndForward());
            _lastChosenStage = stageEntry.Stage;
            return stageInstance;
        }
    }
}