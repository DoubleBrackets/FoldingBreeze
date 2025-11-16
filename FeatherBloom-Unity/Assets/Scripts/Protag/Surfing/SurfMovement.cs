using UnityEngine;

namespace Protag.Surfing
{
    public class SurfMovement : MonoBehaviour
    {
        [Header("Dependencies")]

        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private Transform _body;

        [Header("Config")]

        [SerializeField]
        private AnimationCurve _steeringCurve;

        [SerializeField]
        private float _steeringMaxAngularVelocity;

        [SerializeField]
        private float _moveAcceleration;

        [Tooltip("Speed range at which the player will accelerate")]
        [SerializeField]
        private float _maxMoveSpeed;

        [SerializeField]
        private float _moveSpeedCap;

        [SerializeField]
        private float _minMoveSpeed;

        [SerializeField]
        private float _gravityAccelGround;

        [SerializeField]
        private float _gravityAccelAir;

        public Vector3 CurrentVelocity => _rb.linearVelocity;

        public void Tick(float horizontalAim, GroundChecker.GroundedInfo info, float boost, float deltaTime)
        {
            horizontalAim = _steeringCurve.Evaluate(Mathf.Abs(horizontalAim)) * Mathf.Sign(horizontalAim);
            Vector3 normal = info.GroundNormal;
            Vector3 initialVel = _rb.linearVelocity;
            Vector3 transientVel = initialVel;

            // Calculate horizontal input
            Vector3 currentHVelocity = Vector3.ProjectOnPlane(initialVel, normal);
            Vector3 currentHInput = currentHVelocity.normalized;
            if (currentHInput == Vector3.zero)
            {
                currentHInput = transform.forward;
            }

            // Steering
            float angularVelocity = _steeringMaxAngularVelocity * horizontalAim;
            Vector3 newHInput = Quaternion.AngleAxis(angularVelocity * deltaTime, normal) * currentHInput;

            // Calculate desired horizontal velocity w respect to ground plane
            float currentHSpeed = currentHVelocity.magnitude;

            Vector3 desiredHVel = Vector3.zero;
            if (currentHSpeed >= _maxMoveSpeed)
            {
                desiredHVel = newHInput * currentHSpeed;
            }
            else
            {
                desiredHVel = newHInput * _maxMoveSpeed;
            }

            Debug.DrawLine(_body.position, _body.position + newHInput.normalized, Color.red, 1f);

            // Accelerate
            Vector3 newHVelocity = Vector3.MoveTowards(
                currentHVelocity,
                desiredHVel,
                _moveAcceleration * deltaTime);

            if (boost > 0f)
            {
                Debug.Log($"Boosted {boost}");
                newHVelocity += newHVelocity.normalized * boost;
            }

            if (newHVelocity.magnitude < _minMoveSpeed)
            {
                newHVelocity = newHVelocity.normalized * _minMoveSpeed;
            }

            if (newHVelocity.magnitude > _moveSpeedCap)
            {
                newHVelocity = newHVelocity.normalized * _moveSpeedCap;
            }

            // Apply
            transientVel += newHVelocity - currentHVelocity;

            // Gravity
            float gravityAccel = info.IsGrounded ? _gravityAccelGround : _gravityAccelAir;
            Vector3 gravity = Vector3.down * gravityAccel * deltaTime;
            transientVel += gravity;

            _rb.linearVelocity = transientVel;
        }
    }
}