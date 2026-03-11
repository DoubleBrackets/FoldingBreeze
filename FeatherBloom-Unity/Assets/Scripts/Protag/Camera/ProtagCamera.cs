using Unity.Cinemachine;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Protag.Camera
{
    public class ProtagCamera : MonoBehaviour, IValueSOObserver
    {
        [Header("ValueSO (Read)")]

        [SerializeField]
        private Vector3Value _protagVelocity;

        [SerializeField]
        private FloatValueSO _processedHorizontalInput;

        [Header("Depends")]

        [SerializeField]
        private Transform _cameraTarget;

        [SerializeField]
        private CinemachineThirdPersonFollow _cameraFollow;

        [SerializeField]
        private CinemachineCamera _camera;

        [SerializeField]
        private Transform _aimTransform;

        [Header("Camera")]

        [SerializeField]
        private float _cameraAimSmoothSpeed;

        [SerializeField]
        private float _cameraFovSmoothSpeed;

        [SerializeField]
        private Vector2 _fovRange;

        [SerializeField]
        private Vector2 _velocityForFovRange;

        private Vector3 _lastVelocity;
        private float _horizontalInput;

        private void Awake()
        {
            _protagVelocity.AddListener(this, HandleProtagVelocityChange, true);
            _processedHorizontalInput.AddListener(this, HandleHorizontalInputChange, true);
        }

        private void OnDestroy()
        {
            _protagVelocity.RemoveListener(this);
            _processedHorizontalInput.RemoveListener(this);
        }

        private void HandleProtagVelocityChange(Vector3 vel)
        {
            _lastVelocity = vel;
        }

        private void HandleHorizontalInputChange(float horizontalInput)
        {
            _horizontalInput = horizontalInput;
        }

        private void FixedUpdate()
        {
            UpdateProtagCamera(_horizontalInput, Time.fixedDeltaTime, _lastVelocity);
        }

        private void UpdateProtagCamera(float horizontalInput, float deltaTime, Vector3 currentVelocity)
        {
            float t3 = 1 - Mathf.Pow(0.01f, deltaTime * _cameraAimSmoothSpeed);
            Quaternion currentCameraRotation = _cameraTarget.transform.rotation;
            Quaternion newCameraRotation = Quaternion.Lerp(currentCameraRotation, _aimTransform.rotation, t3);
            _cameraTarget.transform.rotation = newCameraRotation;

            float currentCameraSide = _cameraFollow.CameraSide;
            float cameraSideT = 1 - Mathf.Pow(0.01f, deltaTime * _cameraFovSmoothSpeed);
            float smoothedCameraSide = Mathf.Lerp(currentCameraSide, 0.5f * horizontalInput + 0.5f, cameraSideT);
            _cameraFollow.CameraSide = smoothedCameraSide;

            float speed = currentVelocity.magnitude;
            float speedNormalized = Mathf.InverseLerp(_velocityForFovRange.x, _velocityForFovRange.y, speed);
            float desiredFov = Mathf.Lerp(_fovRange.x, _fovRange.y, speedNormalized);

            float currentFov = _camera.Lens.FieldOfView;
            float fovT = 1 - Mathf.Pow(0.01f, deltaTime * _cameraFovSmoothSpeed);
            desiredFov = Mathf.Lerp(currentFov, desiredFov, fovT);
            _camera.Lens.FieldOfView = desiredFov;
        }
    }
}