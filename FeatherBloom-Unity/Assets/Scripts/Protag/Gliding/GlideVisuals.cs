using UnityEngine;

namespace Protag.Gliding
{
    public class GlideVisuals : MonoBehaviour
    {
        [Header("Depends")]

        [SerializeField]
        private Transform _aimPivot;

        [SerializeField]
        private Transform _rollPivot;

        [Header("Config")]

        [SerializeField]
        private float _maxRollTiltAngle;

        [SerializeField]
        private float _aimLerpFactor;

        [SerializeField]
        private float _rollLerpFactor;

        private float _currentRollLeanAngle;

        public void UpdateVisuals(float horizontalInput, float verticalInput, Vector3 currentVelocity, float deltaTime)
        {
            float t1 = 1 - Mathf.Pow(0.01f, deltaTime * _aimLerpFactor);
            Quaternion desiredRotation = Quaternion.LookRotation(currentVelocity, Vector3.up);
            Quaternion currentRotation = _aimPivot.rotation;
            Quaternion newRotation = Quaternion.Lerp(currentRotation, desiredRotation, t1);

            _aimPivot.rotation = newRotation;

            float t2 = 1 - Mathf.Pow(0.01f, deltaTime * _rollLerpFactor);
            _currentRollLeanAngle = Mathf.Lerp(_currentRollLeanAngle, -horizontalInput * _maxRollTiltAngle, t2);
            _rollPivot.localRotation = Quaternion.Euler(0, 0, _currentRollLeanAngle);
        }
    }
}