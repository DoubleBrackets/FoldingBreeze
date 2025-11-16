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
            SceneManager.LoadScene(sceneName);
        }
    }
}