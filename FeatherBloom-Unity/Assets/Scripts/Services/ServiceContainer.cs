using UnityEngine;

namespace Services
{
    public class ServiceContainer : MonoBehaviour
    {
        private static ServiceContainer _instance;

        [SerializeField]
        private GameObject _servicePrefab;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                if (_servicePrefab != null)
                {
                    Instantiate(_servicePrefab, transform);
                }

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}