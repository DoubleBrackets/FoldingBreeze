using Cysharp.Threading.Tasks;
using DevTools;
using Framework.GlobalServices;
using Framework.LevelLoading;
using Framework.Timescaling;
using Input;
using NaughtyAttributes;
using Protag;
using Protag.LevelGen;
using Protag.LevelGen.StageRoster;
using UnityEngine;
using ValueSO.Core;

namespace Framework.Gameplay
{
    /// <summary>
    ///     Top level manager for gameplay scene
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private StageRosterSO _stageRosterSO;

        [SerializeField]
        private Protaganist _protagInScene;

        [SerializeField]
        private GameLevelSO _scoreLevel;

        [SerializeField]
        private GameLevelSO _gameplayLevel;

        [SerializeField]
        private Transform _spawnPoint;

        [SerializeField]
        private Transform _stageParent;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private FloatValueSO _killHeight;

        private MapService _mapService;
        private ScoreService _scoreService;
        private LevelLoader _levelLoader;

        private void Awake()
        {
            _scoreService = ServiceLocator.GetService<ScoreService>();
            _levelLoader = ServiceLocator.GetService<LevelLoader>();

            _mapService = new MapService(_stageParent, new StageSelector(_stageRosterSO));
            _protagInScene.Initialize(
                ServiceLocator.GetService<TimeScaleService>(),
                ServiceLocator.GetService<GameplayInputService>());

            _protagInScene.OnDeath += HandleProtagDeath;

            if (!DevToolState.DoNotLoadMapOnStart)
            {
                _mapService.StartPlayingMap(FindAnyObjectByType<MapStage>());
            }

            MoveProtagToSpawnPoint();
            _scoreService.ResetScore();
        }

        private void OnDestroy()
        {
            _protagInScene.OnDeath -= HandleProtagDeath;
        }

        private void Update()
        {
            MapUpdateResult updateResult = _mapService.UpdateInfo(_protagInScene.Position);

            _killHeight.SetValue(updateResult.KillHeight);

            if (updateResult.DidMoveToNextStage)
            {
                _scoreService.AddScore(1);
            }

            if (updateResult.ShouldKillPlayer)
            {
                _protagInScene.Kill();
            }
        }

        private void HandleProtagDeath()
        {
            if (DevToolState.AutoRestartOnDeath)
            {
                _levelLoader.LoadLevel(_gameplayLevel).Forget();
            }
            else
            {
                _levelLoader.LoadLevel(_scoreLevel).Forget();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.right * 1000f);

            // Draw spawn point
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_spawnPoint.position, 0.5f);
            Gizmos.DrawLine(_spawnPoint.position, _spawnPoint.position + _spawnPoint.forward * 5f);
        }

        [Button("Move Protag to Spawn Point")]
        private void MoveProtagToSpawnPoint()
        {
            _protagInScene.SetPositionAndDirection(_spawnPoint.position, _spawnPoint.forward);
        }
    }
}