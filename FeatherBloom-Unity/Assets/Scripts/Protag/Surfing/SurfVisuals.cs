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

        [Header("Config")]

        [SerializeField]
        private float _aimSmoothSpeed;

        [SerializeField]
        private float _tiltSmoothSpeed;

        [SerializeField]
        private float _maxTiltAngle;

        private float _currentLeanRotation;

        public void UpdateSurfVisuals(GroundChecker.GroundedInfo info, Vector3 currentVelocity, float horizontalInput,
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
        }
    }
}