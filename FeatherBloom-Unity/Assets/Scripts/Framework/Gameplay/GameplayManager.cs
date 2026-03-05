using Cysharp.Threading.Tasks;
using DevTools;
using Framework.GlobalServices;
using Framework.LevelLoading;
using Protag;
using Protag.LevelGen;
using UnityEngine;

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

        private MapService _mapService;
        private ScoreService _scoreService;
        private LevelLoader _levelLoader;

        private void Awake()
        {
            _scoreService = ServiceLocator.GetService<ScoreService>();
            _levelLoader = ServiceLocator.GetService<LevelLoader>();

            _mapService = new MapService(transform, _stageRosterSO, _protagInScene);

            _mapService.OnStageProgressed += HandleStageProgressed;
            _protagInScene.OnDeath += HandleProtagDeath;

            if (!DevToolState.DoNotLoadMapOnStart)
            {
                _mapService.StartPlayingMap();
            }

            _protagInScene.SetPositionAndDirection(Vector3.zero, Vector3.forward);
            _scoreService.ResetScore();
        }

        private void OnDestroy()
        {
            _mapService.OnStageProgressed -= HandleStageProgressed;
            _protagInScene.OnDeath -= HandleProtagDeath;
        }

        private void HandleStageProgressed()
        {
            _scoreService.AddScore(1);
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
    }
}