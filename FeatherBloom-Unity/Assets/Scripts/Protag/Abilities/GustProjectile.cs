using Cysharp.Threading.Tasks;
using Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace Protag.Abilities
{
    public class GustProjectile : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _cleanupTime;

        [SerializeField]
        private float _angularVelocity;

        [SerializeField]
        private float _maxSpeed;

        [SerializeField]
        private float _impactDistance;

        [Header("Events")]

        [SerializeField]
        private UnityEvent _onImpact;

        [SerializeField]
        private UnityEvent _onCleanup;

        private Corruption _target;

        private bool _hit;

        private void FixedUpdate()
        {
            if (_hit)
            {
                return;
            }

            if (_target == null)
            {
                CleanupTimer().Forget();
                return;
            }

            Vector3 vectorToTarget = _target.Position - transform.position;

            if (vectorToTarget.magnitude <= _impactDistance)
            {
                _hit = true;
                _target.BlowAway();
                _onImpact?.Invoke();
                CleanupTimer().Forget();
                return;
            }

            Vector3 linearVel = _rigidbody.linearVelocity;

            Quaternion angle = Quaternion.LookRotation(linearVel, Vector3.up);
            Quaternion desiredAngle = Quaternion.LookRotation(vectorToTarget.normalized, Vector3.up);
            Quaternion newAngle = Quaternion.RotateTowards(angle, desiredAngle, _angularVelocity * Time.fixedDeltaTime);

            transform.rotation = newAngle;

            _rigidbody.linearVelocity =
                newAngle * Vector3.forward * _maxSpeed;
        }

        public void Initialize(Corruption target, Vector3 initialDirection)
        {
            _target = target;
            _rigidbody.linearVelocity = initialDirection * _maxSpeed;
        }

        private async UniTaskVoid CleanupTimer()
        {
            await UniTask.WaitForSeconds(_cleanupTime);
            _onCleanup?.Invoke();
            Destroy(gameObject);
        }
    }
}