using DevTools;
using Protag.LevelGen.StageRoster;
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

        private Transform _stageParent;
        private StageSelector _stageSelector;

        /// <summary>
        ///     Angular position used to place the current stage
        /// </summary>
        private float _currentStagePlacementAngularPosition;

        /// <summary>
        ///     Y position used to place the current stage
        /// </summary>
        private float _currentStagePlacementPositionY;

        /// <summary>
        ///     How many times the player has looped around
        /// </summary>
        private int _playerLoopCount;

        private float _previousPlayerAngularPos;

        public MapService(Transform stageParent, StageSelector stageSelector)
        {
            _stageParent = stageParent;
            _stageSelector = stageSelector;
        }

        public void StartPlayingMap(MapStage initialStage = null)
        {
            InitializeMap(initialStage);
        }

        private void InitializeMap(MapStage initialStage)
        {
            // Load the first two stages
            // If a stage is in the scene when loading, use it as the initial stage
            if (initialStage != null)
            {
                initialStage.Initialize(_currentStagePlacementPositionY, _currentStagePlacementAngularPosition);
                _nextMapStage = initialStage;
            }
            else
            {
                _nextMapStage = InstantiateStageAtCurrentPosition(_stageSelector.GetNextStage(), false);
            }

            MoveToNextStage();
        }

        private void MoveToNextStage()
        {
            // Destroy previous
            if (_previousMapStage)
            {
                Debug.Log($"Destroying previous stage: {_previousMapStage.name}");
                GameObject.Destroy(_previousMapStage.gameObject);
            }

            _previousMapStage = _currentMapStage;

            if (_currentMapStage)
            {
                _currentStagePlacementPositionY += _currentMapStage.Height;
                _currentStagePlacementAngularPosition += _currentMapStage.StageAngularWidth;
            }

            OnGUIHook.SetElement("Stage Placement Y", _currentStagePlacementPositionY.ToString());

            _currentMapStage = _nextMapStage;
            Debug.Log($"Moved to new current stage: {_currentMapStage.name}");

            _nextMapStage = InstantiateStageAtNextPosition(_stageSelector.GetNextStage(), true);
            Debug.Log($"Instantiated new stage: {_nextMapStage.name}");
        }

        public MapUpdateResult UpdateInfo(Vector3 protagPos)
        {
            var shouldKillProtag = false;
            var didProgressStage = false;
            float killHeight = _currentStagePlacementPositionY + _currentMapStage.KillZoneVerticalPos;
            if (protagPos.y < killHeight)
            {
                shouldKillProtag = true;
            }

            float protagAngularPos = Mathf.Atan2(protagPos.z, protagPos.x) * Mathf.Rad2Deg;

            if (protagAngularPos < -90 && _previousPlayerAngularPos > 90)
            {
                // Player has looped around
                _playerLoopCount++;
            }

            _previousPlayerAngularPos = protagAngularPos;

            float currentStageEndAngle = _currentMapStage.EndingAngularPos;
            float trueProtagAngularPos = protagAngularPos + _playerLoopCount * 360f;

            OnGUIHook.SetElement("Player Loop Count", _playerLoopCount.ToString());
            OnGUIHook.SetElement("Protag Angular Pos", protagAngularPos.ToString());
            OnGUIHook.SetElement("True Protag Angular Pos", trueProtagAngularPos.ToString());
            OnGUIHook.SetElement("Current Stage End Angle", currentStageEndAngle.ToString());
            OnGUIHook.SetElement("Kill Height", killHeight.ToString());
            OnGUIHook.SetElement("Protag Y Pos", protagPos.y.ToString());

            if (trueProtagAngularPos > currentStageEndAngle)
            {
                didProgressStage = true;
                MoveToNextStage();
            }

            return new MapUpdateResult
            {
                ShouldKillPlayer = shouldKillProtag,
                DidMoveToNextStage = didProgressStage,
                KillHeight = killHeight
            };
        }

        private MapStage InstantiateStageAtCurrentPosition(StageSO stage, bool riseAnim)
        {
            return InstantiateStage(
                stage,
                _currentStagePlacementPositionY,
                _currentStagePlacementAngularPosition,
                riseAnim);
        }

        private MapStage InstantiateStageAtNextPosition(StageSO stage, bool riseAnim)
        {
            return InstantiateStage(
                stage,
                _currentStagePlacementPositionY + _currentMapStage.Height,
                _currentStagePlacementAngularPosition + _currentMapStage.StageAngularWidth,
                riseAnim);
        }

        private MapStage InstantiateStage(StageSO stage,
            float stageStartPositionY,
            float stageAngularStartPosition,
            bool riseAnim)
        {
            MapStage stagePrefab = stage.StagePrefab;
            MapStage stageInstance = GameObject.Instantiate(stagePrefab, _stageParent);
            stageInstance.Initialize(stageStartPositionY, stageAngularStartPosition, riseAnim);
            return stageInstance;
        }
    }
}