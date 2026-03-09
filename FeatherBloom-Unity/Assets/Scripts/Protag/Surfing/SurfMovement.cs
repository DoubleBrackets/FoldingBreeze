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

        public Vector3 CurrentVelocity => _rb.linearVelocity;

        public void Tick(float horizontalAim,
            GroundChecker.GroundedInfo info,
            SurfConfigSO config,
            float boost,
            float deltaTime)
        {
            horizontalAim = config.SteeringCurve.Evaluate(Mathf.Abs(horizontalAim)) * Mathf.Sign(horizontalAim);
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
            float angularVelocity = config.SteeringMaxAngularVelocity * horizontalAim;
            Vector3 newHInput = Quaternion.AngleAxis(angularVelocity * deltaTime, normal) * currentHInput;

            // Calculate desired horizontal velocity w respect to ground plane
            float currentHSpeed = currentHVelocity.magnitude;

            Vector3 desiredHVel = Vector3.zero;
            if (currentHSpeed >= config.MaxMoveSpeed)
            {
                desiredHVel = newHInput * currentHSpeed;
            }
            else
            {
                desiredHVel = newHInput * config.MaxMoveSpeed;
            }

            Debug.DrawLine(_body.position, _body.position + newHInput.normalized, Color.red, 1f);

            // Accelerate
            Vector3 newHVelocity = Vector3.MoveTowards(
                currentHVelocity,
                desiredHVel,
                config.MoveAcceleration * deltaTime);

            if (boost > 0f)
            {
                Debug.Log($"Boosted {boost}");
                newHVelocity += newHVelocity.normalized * boost;
            }

            if (newHVelocity.magnitude < config.MinMoveSpeed)
            {
                newHVelocity = newHVelocity.normalized * config.MinMoveSpeed;
            }

            if (newHVelocity.magnitude > config.MoveSpeedCap)
            {
                newHVelocity = newHVelocity.normalized * config.MoveSpeedCap;
            }

            // Apply
            transientVel += newHVelocity - currentHVelocity;

            // Gravity
            float gravityAccel = info.IsGrounded ? config.GravityAccelGround : config.GravityAccelAir;
            Vector3 gravity = Vector3.down * gravityAccel * deltaTime;
            transientVel += gravity;

            _rb.linearVelocity = transientVel;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rb.linearVelocity = velocity;
        }
    }
}