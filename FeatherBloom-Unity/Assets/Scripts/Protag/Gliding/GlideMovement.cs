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

        public Vector3 CurrentVelocity => _rb.linearVelocity;

        public void Tick(Vector2 aim, GlideConfigSO config, float deltaTime)
        {
            float tiltSteer = config.TiltSteerCurve.Evaluate(Mathf.Abs(aim.y)) * Mathf.Sign(aim.y);
            float rollSteer = config.RollSteerCurve.Evaluate(Mathf.Abs(aim.x)) * Mathf.Sign(aim.x);

            Vector3 currentVel = _rb.linearVelocity;
            float currentSpeed = currentVel.magnitude;

            Vector3 currentTrajectoryDirection = currentVel.normalized;

            // ROLL
            Vector3 currentHorizontalTrajectory = currentTrajectoryDirection;
            currentHorizontalTrajectory.y = 0;

            Vector3 targetHorizontalTrajectory =
                Quaternion.AngleAxis(rollSteer * config.RollMaxAngularVelocity * deltaTime, Vector3.up) *
                currentHorizontalTrajectory.normalized;

            Vector3 right = Vector3.Cross(currentTrajectoryDirection, Vector3.up).normalized;

            // TILT

            // -90 to 90
            float currentTilt = -Vector3.Angle(currentTrajectoryDirection, Vector3.up) + 90f;
            float newTilt = currentTilt + tiltSteer * config.TiltMaxAngularVelocity * deltaTime;
            newTilt = Mathf.Clamp(newTilt, config.TiltBoundLower, config.TiltBoundUpper);
            Vector3 newTargetTrajectory = Quaternion.AngleAxis(newTilt, right) *
                                          targetHorizontalTrajectory;

            Vector3 targetVelocity = newTargetTrajectory * currentSpeed;

            float dragT = Mathf.Pow(config.Drag, deltaTime);
            // Drag linear deaccel
            targetVelocity = targetVelocity.normalized * Mathf.Max(0,
                targetVelocity.magnitude * dragT);

            // Apply gravity
            float gravityRatio = Vector3.Dot(targetVelocity.normalized, Vector3.down);

            if (currentVel.magnitude > config.MinFlightSpeed)
            {
                targetVelocity = targetVelocity.normalized *
                                 (targetVelocity.magnitude + gravityRatio * config.GravityAccel * deltaTime);
            }
            else
            {
                targetVelocity += Vector3.down * config.GravityAccel * deltaTime;
            }

            targetVelocity += Vector3.down * config.FixedGravityAccel * deltaTime;

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