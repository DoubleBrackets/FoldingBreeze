using Unity.Cinemachine;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Protag.Camera
{
    public class ProtagCameraTogger : MonoBehaviour, IValueSOObserver
    {
        [Header("ValueSO")]

        [SerializeField]
        private BoolValueSO _isCameraActive;

        [SerializeField]
        private bool _invert;

        [SerializeField]
        private CinemachineCamera _camera;

        private void Awake()
        {
            _isCameraActive.AddListener(this, HandleCameraActiveChange, true);
        }

        private void OnDestroy()
        {
            _isCameraActive.RemoveListener(this);
        }

        private void HandleCameraActiveChange(bool isActive)
        {
            _camera.enabled = isActive ^ _invert;
        }
    }
}