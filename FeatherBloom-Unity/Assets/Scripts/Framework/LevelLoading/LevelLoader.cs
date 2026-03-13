using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Framework.LevelLoading.LevelTransition;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.LevelLoading
{
    public class LevelLoader
    {
        private GameLevelSO _currentGameLevel;
        private GameLevelSO _previousGameLevel;

        /// <summary>
        ///     Queue for levels to be loaded, in case a transition is interrupted by another level load
        /// </summary>
        private Queue<GameLevelSO> _levelLoadQueue = new();

        private bool _isLoading;

        private readonly ILevelLoadTransition _levelLoadTransition;

        public LevelLoader(ILevelLoadTransition levelLoadTransition)
        {
            _levelLoadTransition = levelLoadTransition;
        }

        /// <summary>
        ///     Loads a new level, unloading the current one if necessary
        /// </summary>
        /// <param name="levelToLoad"></param>
        public async UniTask LoadLevel(GameLevelSO levelToLoad)
        {
            Debug.Log($"Loading level {levelToLoad.name}");
            if (_isLoading)
            {
                if (_levelLoadQueue.Count < 1)
                {
                    Debug.Log("Level is already loading, adding to queue");
                    _levelLoadQueue.Enqueue(levelToLoad);
                }
                else
                {
                    Debug.LogWarning("Too many queued loads, ignoring load request");
                }

                return;
            }

            _isLoading = true;

            await _levelLoadTransition.TransitionOutFromCurrentScene();

            // Debug.Log($"Killed {DOTween.KillAll()} tweens");

            string sceneName = levelToLoad.SceneName;
            string currentSceneName = _currentGameLevel != null ? _currentGameLevel.SceneName : string.Empty;

            if (SceneManager.GetSceneByName(currentSceneName).isLoaded)
            {
                await SceneManager.UnloadSceneAsync(currentSceneName);
            }

            // Load the new scene
            Debug.Log($"Loading scene {sceneName}");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            _previousGameLevel = _currentGameLevel;
            _currentGameLevel = levelToLoad;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            await _levelLoadTransition.TransitionInToNewScene();

            _isLoading = false;

            // Chain with any queued level loads
            if (_levelLoadQueue.Count > 0)
            {
                GameLevelSO nextLevel = _levelLoadQueue.Dequeue();
                Debug.Log($"Loading next level from queue: {nextLevel.name}");
                await LoadLevel(nextLevel);
            }
        }
    }
}