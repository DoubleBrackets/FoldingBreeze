using Events;
using Interactables;
using UnityEngine;

namespace Protag.Abilities
{
    public class GustAbility : MonoBehaviour
    {
        [SerializeField]
        private GustProjectile _gustPrefab;

        [SerializeField]
        private LayerMask _corruptionLayerMask;

        [SerializeField]
        private float _targetRadius;

        [SerializeField]
        private Transform _emitPoint;

        [SerializeField]
        private Vector3 _emitAngle;

        [Header("Event Out")]

        [SerializeField]
        private VoidEvent _onGust;

        private void OnDrawGizmos()
        {
            if (_emitPoint)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(_emitPoint.position, _targetRadius);
            }
        }

        public bool TryGust()
        {
            // Spherecast
            Collider[] sphereCast = Physics.OverlapSphere(_emitPoint.position, _targetRadius, _corruptionLayerMask);

            var minDist = float.MaxValue;
            Corruption targetCorruption = null;
            foreach (Collider sphere in sphereCast)
            {
                var corruption = sphere.GetComponentInParent<Corruption>();
                if (corruption && !corruption.IsAlreadyTargeted)
                {
                    float dist = (corruption.Position - _emitPoint.position).magnitude;
                    if (dist < minDist)
                    {
                        minDist = dist;
                        targetCorruption = corruption;
                    }
                }
            }

            if (targetCorruption)
            {
                GustProjectile gustProjectile = Instantiate(_gustPrefab, _emitPoint.position, _emitPoint.rotation);

                gustProjectile.Initialize(targetCorruption, _emitPoint.TransformDirection(_emitAngle.normalized));
                _onGust?.Raise();
                targetCorruption.MarkAsTargeted();
                return true;
            }

            return false;
        }
    }
}