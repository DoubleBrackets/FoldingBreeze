using UnityEngine;

namespace Protag
{
    public class GroundChecker : MonoBehaviour
    {
        public struct GroundedInfo
        {
            public bool IsGrounded;
            public Vector3 GroundNormal;
        }

        [Header("Dependencies")]

        [SerializeField]
        private Rigidbody _rb;

        [Header("GroundConfig")]

        [SerializeField]
        private float _groundCheckOffset;

        [SerializeField]
        private float _groundCheckDistance;

        [SerializeField]
        private LayerMask _groundLayerMask;

        [SerializeField]
        private float _maxGroundAngle;

        private float _forceUngroundTime;

        private void OnDrawGizmos()
        {
            Vector3 pos = _rb.position + _groundCheckOffset * Vector3.up;
            Vector3 down = Vector3.down;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                pos,
                pos + _groundCheckDistance * down);
        }

        public void ForceUnground(float duration)
        {
            float newForceUngroundTime = Time.time + duration;

            if (newForceUngroundTime > _forceUngroundTime)
            {
                _forceUngroundTime = newForceUngroundTime;
            }
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

            Debug.DrawLine(
                hit.point,
                hit.point + hit.normal,
                isGrounded ? Color.green : Color.red, 0.25f);

            if (Time.time < _forceUngroundTime)
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