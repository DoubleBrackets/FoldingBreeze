using UnityEngine;

namespace Protag.Gliding
{
    public class GlideMovement : MonoBehaviour
    {
        [Header("Dependencies")]

        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private Transform _body;

        [Header("Config")]

        [SerializeField]
        private AnimationCurve _tiltSteerCurve;

        [SerializeField]
        private AnimationCurve _rollSteerCurve;

        [SerializeField]
        private float _tiltMaxAngularVelocity;

        [SerializeField]
        private float _rollMaxAngularVelocity;

        [SerializeField]
        private float _gravityAccel;

        [SerializeField]
        private float _drag;

        public Vector3 CurrentVelocity => _rb.linearVelocity;

        public void Tick(float horizontalAim, float verticalAim, float deltaTime)
        {
            float tiltSteer = _tiltSteerCurve.Evaluate(Mathf.Abs(verticalAim)) * Mathf.Sign(verticalAim);
            float rollSteer = _rollSteerCurve.Evaluate(Mathf.Abs(horizontalAim)) * Mathf.Sign(horizontalAim);

            Vector3 currentVel = _rb.linearVelocity;
            float currentSpeed = currentVel.magnitude;

            Vector3 currentAimDirection = currentVel.normalized;
            // Roll
            Vector3 targetAimDirection =
                Quaternion.AngleAxis(rollSteer * _rollMaxAngularVelocity * deltaTime, Vector3.up) * currentAimDirection;

            Vector3 right = Vector3.Cross(currentAimDirection, Vector3.up).normalized;
            // Tilt
            targetAimDirection = Quaternion.AngleAxis(tiltSteer * _tiltMaxAngularVelocity * deltaTime, right) *
                                 targetAimDirection;

            Vector3 targetVelocity = targetAimDirection * currentSpeed;

            // Drag linear deaccel
            targetVelocity = targetVelocity.normalized * Mathf.Max(0,
                targetVelocity.magnitude - _drag * deltaTime);

            // Apply gravity
            targetVelocity += Vector3.down * _gravityAccel * deltaTime;

            _rb.linearVelocity = targetVelocity;
        }

        public void Boost(float amount)
        {
            Vector3 currentVel = _rb.linearVelocity;
            Vector3 boostedVel = currentVel.normalized * (currentVel.magnitude + amount);
            _rb.linearVelocity = boostedVel;
        }
    }
}