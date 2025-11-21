using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneChangeMono : MonoBehaviour
    {
        [Scene]
        [SerializeField]
        private string sceneName;

        public void ChangeScene()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Scene name is not set or is empty.");
                return;
            }

            // Use Unity's SceneManager to load the scene
            ChangeSceneDelay().Forget();
        }

        private async UniTaskVoid ChangeSceneDelay()
        {
            await UniTask.Yield();
            SceneManager.LoadScene(sceneName);
        }
    }
}