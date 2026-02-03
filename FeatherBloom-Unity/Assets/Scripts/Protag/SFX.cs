using FMODUnity;
using UnityEngine;

namespace Protag
{
    public class ProtagSFX : MonoBehaviour
    {
        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private Rigidbody _rb;

        [SerializeField]
        private Vector2 _slideVelocityRange;

        [SerializeField]
        private Vector2 _glideVelocityRange;

        [SerializeField]
        private StudioEventEmitter _slideSFX;

        private bool wasGrounded;

        private void Update()
        {
            Vector3 vel = _rb.linearVelocity;
            float slideT = Mathf.InverseLerp(_slideVelocityRange.x, _slideVelocityRange.y, vel.magnitude);
            float glideT = Mathf.InverseLerp(_glideVelocityRange.x, _glideVelocityRange.y, vel.magnitude);

            RuntimeManager.StudioSystem.setParameterByName("SlidingSpeed", slideT);
            RuntimeManager.StudioSystem.setParameterByName("GlidingSpeed", glideT);

            bool isGrounded = _groundChecker.CheckGrounded().IsGrounded;

            if (wasGrounded && !isGrounded)
            {
                _slideSFX.Stop();
            }
            else if (!wasGrounded && isGrounded)
            {
                _slideSFX.Play();
            }

            wasGrounded = isGrounded;

            RuntimeManager.StudioSystem.setParameterByName("IsGrounded", isGrounded ? 1 : 0);
        }
    }
}