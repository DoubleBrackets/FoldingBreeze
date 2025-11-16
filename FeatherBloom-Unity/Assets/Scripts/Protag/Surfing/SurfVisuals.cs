using Unity.Cinemachine;
using UnityEngine;

namespace Protag.Surfing
{
    public class SurfVisuals : MonoBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Transform _aimPivot;

        [SerializeField]
        private Transform _leanPivotTransform;

        [SerializeField]
        private Transform _cameraTarget;

        [SerializeField]
        private CinemachineThirdPersonFollow _cameraFollow;

        [SerializeField]
        private CinemachineCamera _camera;

        [Header("Config")]

        [SerializeField]
        private float _aimSmoothSpeed;

        [SerializeField]
        private float _tiltSmoothSpeed;

        [SerializeField]
        private float _maxTiltAngle;

        [Header("Camera")]

        [SerializeField]
        private float _cameraAimSmoothSpeed;

        [SerializeField]
        private float _cameraFovSmoothSpeed;

        [SerializeField]
        private Vector2 _fovRange;

        [SerializeField]
        private Vector2 _velocityForFovRange;

        private float _currentLeanRotation;

        public void UpdateSurfVisuals(SurfMovement.GroundedInfo info, Vector3 currentVelocity, float horizontalInput,
            float deltaTime)
        {
            float t1 = 1 - Mathf.Pow(0.01f, deltaTime * _aimSmoothSpeed);
            Vector3 right = Vector3.Cross(info.GroundNormal, currentVelocity.normalized);
            Vector3 orthoForward = Vector3.Cross(right, info.GroundNormal);
            Quaternion desiredRotation = Quaternion.LookRotation(orthoForward, info.GroundNormal);
            Quaternion currentRotation = _aimPivot.rotation;
            Quaternion newRotation = Quaternion.Lerp(currentRotation, desiredRotation, t1);

            _aimPivot.rotation = newRotation;

            float t2 = 1 - Mathf.Pow(0.01f, deltaTime * _tiltSmoothSpeed);
            _currentLeanRotation = Mathf.Lerp(_currentLeanRotation, -horizontalInput * _maxTiltAngle, t2);
            _leanPivotTransform.localRotation = Quaternion.Euler(0, 0, _currentLeanRotation);

            float t3 = 1 - Mathf.Pow(0.01f, deltaTime * _cameraAimSmoothSpeed);
            Quaternion currentCameraRotation = _cameraTarget.transform.rotation;
            Quaternion newCameraRotation = Quaternion.Lerp(currentCameraRotation, desiredRotation, t3);
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