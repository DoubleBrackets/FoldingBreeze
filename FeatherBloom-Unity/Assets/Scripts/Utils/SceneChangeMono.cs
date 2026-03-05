using Cysharp.Threading.Tasks;
using Framework;
using Framework.LevelLoading;
using UnityEngine;

namespace Utils
{
    public class SceneChangeMono : MonoBehaviour
    {
        [SerializeField]
        private GameLevelSO _gameLevel;

        public void ChangeScene()
        {
            if (_gameLevel == null)
            {
                Debug.LogError("Game level is not set.");
                return;
            }

            ChangeSceneDelay().Forget();
        }

        private async UniTaskVoid ChangeSceneDelay()
        {
            await UniTask.Yield();
            ServiceLocator.GetService<LevelLoader>().LoadLevel(_gameLevel).Forget();
        }
    }
}