using UnityEngine;

namespace Protag.Surfing
{
    public class SurfMovement : MonoBehaviour
    {
        public struct GroundedInfo
        {
            public bool IsGrounded;
            public Vector3 GroundNormal;
        }

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
        private float _gravityAccel;

        [Header("GroundConfig")]

        [SerializeField]
        private float _groundCheckOffset;

        [SerializeField]
        private float _groundCheckDistance;

        [SerializeField]
        private LayerMask _groundLayerMask;

        [SerializeField]
        private float _maxGroundAngle;

        public Vector3 CurrentVelocity => _rb.linearVelocity;

        private void OnDrawGizmos()
        {
            Vector3 pos = _rb.position + _groundCheckOffset * Vector3.up;
            Vector3 down = Vector3.down;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                pos,
                pos + _groundCheckDistance * down);
        }

        public void Tick(float horizontalAim, GroundedInfo info, float deltaTime)
        {
            horizontalAim = _steeringCurve.Evaluate(Mathf.Abs(horizontalAim)) * Mathf.Sign(horizontalAim);
            Vector3 normal = info.GroundNormal;
            Vector3 initialVel = _rb.linearVelocity;
            Vector3 transientVel = initialVel;

            // Calculate horizontal input w respect to ground plane
            Vector3 currentHInput = new Vector3(initialVel.x, 0, initialVel.z).normalized;
            if (currentHInput == Vector3.zero)
            {
                currentHInput = transform.forward;
            }

            float angularVelocity = _steeringMaxAngularVelocity * horizontalAim;
            Vector3 newHInput = Quaternion.AngleAxis(angularVelocity * deltaTime, Vector3.up) * currentHInput;

            // Calculate desired horizontal velocity w respect to ground plane
            Vector3 currentHVelocity = Vector3.ProjectOnPlane(initialVel, normal);
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

            // Accelerate
            Vector3 newHVelocity = Vector3.MoveTowards(
                currentHVelocity,
                desiredHVel,
                _moveAcceleration * deltaTime);

            // Apply
            transientVel += newHVelocity - currentHVelocity;

            // Gravity
            Vector3 gravity = Vector3.down * _gravityAccel * deltaTime;
            transientVel += gravity;

            _rb.linearVelocity = transientVel;
        }

        public GroundedInfo CheckGrounded()
        {
            Vector3 pos = _rb.position;
            RaycastHit hit;
            bool isGrounded = Physics.Raycast(
                pos + _groundCheckOffset * Vector3.up,
                Vector3.down,
                out hit,
                _groundCheckDistance,
                _groundLayerMask);

            float groundAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (isGrounded && groundAngle > _maxGroundAngle)
            {
                isGrounded = false;
            }

            return new GroundedInfo
            {
                IsGrounded = isGrounded,
                GroundNormal = isGrounded ? hit.normal : Vector3.up
            };
        }
    }
}