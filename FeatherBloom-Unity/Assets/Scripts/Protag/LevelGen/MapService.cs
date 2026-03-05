using System;
using UnityEngine;

namespace Protag.LevelGen
{
    /// <summary>
    ///     Handles simple map generation by connecting stages
    /// </summary>
    public class MapService
    {
        private MapStage _currentMapStage;
        private MapStage _nextMapStage;
        private MapStage _previousMapStage;

        private StageSO _lastChosenStage;

        private StageRosterSO _stageRosterSO;
        private Transform _stageParent;
        private Protaganist _protag;

        public event Action OnStageProgressed;

        public MapService(Transform stageParent, StageRosterSO stageRosterSO, Protaganist protag)
        {
            _stageParent = stageParent;
            _stageRosterSO = stageRosterSO;
            _protag = protag;
        }

        public void StartPlayingMap()
        {
            InitializeMap();
        }

        private StageRosterSO.RosterEntry GetRandomStageEntry()
        {
            return _stageRosterSO.GetRandomStageEntry(_lastChosenStage);
        }

        private void InitializeMap()
        {
            _currentMapStage = LoadInitialStage(_stageRosterSO.GetStartingStageEntry());
            _currentMapStage.SetStageEnabled(true);
            _nextMapStage = LoadStage(GetRandomStageEntry(), _currentMapStage, false);
            _nextMapStage.OnStageSectionEntered.AddListener(HandleOnNextStageEntered);
        }

        private MapStage LoadInitialStage(StageRosterSO.RosterEntry stageEntry)
        {
            MapStage stagePrefab = stageEntry.Prefab;
            MapStage stageInstance = GameObject.Instantiate(stagePrefab, _stageParent);
            stageInstance.Initialize(Vector3.zero, Vector3.forward, _protag);
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
                GameObject.Destroy(_previousMapStage.gameObject);
            }

            _currentMapStage.SetStageEnabled(false);

            // Update
            _previousMapStage = _currentMapStage;
            _currentMapStage = _nextMapStage;
            Debug.Log($"Moved to new current stage: {_currentMapStage.name}");

            _currentMapStage.SetStageEnabled(true);

            _nextMapStage = LoadStage(GetRandomStageEntry(), _currentMapStage, true);
            Debug.Log($"Loaded new stage: {_nextMapStage.name}");

            // Subscribe
            _nextMapStage.OnStageSectionEntered.AddListener(HandleOnNextStageEntered);

            OnStageProgressed?.Invoke();
        }

        private void HandleOnNextStageEntered()
        {
            MoveToNextStage();
        }

        private MapStage LoadStage(StageRosterSO.RosterEntry stageEntry, MapStage previousStageInstance, bool riseAnim)
        {
            MapStage stagePrefab = stageEntry.Prefab;
            MapStage stageInstance = GameObject.Instantiate(stagePrefab, _stageParent);
            stageInstance.Initialize(previousStageInstance.GetEndPosition(), previousStageInstance.GetEndForward(),
                _protag, riseAnim);
            _lastChosenStage = stageEntry.Stage;
            return stageInstance;
        }
    }
}