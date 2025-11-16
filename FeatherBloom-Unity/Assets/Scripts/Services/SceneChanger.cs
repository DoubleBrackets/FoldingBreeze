using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class SceneChanger : MonoBehaviour
    {
        public static SceneChanger Instance { get; private set; }

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
        }

        public void ChangeScene(string sceneName)
        {
            // Use Unity's SceneManager to load the scene
            SceneManager.LoadScene(sceneName);
        }
    }
}