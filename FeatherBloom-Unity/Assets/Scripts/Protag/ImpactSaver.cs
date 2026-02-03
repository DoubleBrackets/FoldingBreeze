using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Protag
{
    /// <summary>
    ///     Saves data from impacts
    /// </summary>
    public class ImpactSaver : MonoBehaviour
    {
        public struct ImpactInfo
        {
            public Vector3 Impulse;
        }

        [SerializeField]
        private LayerMask _terrainLayerMask;

        public UnityEvent<ImpactInfo> OnTerrainImpact;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer.IsInMask(_terrainLayerMask))
            {
                var impactInfo = new ImpactInfo
                {
                    Impulse = other.impulse
                };
                OnTerrainImpact?.Invoke(impactInfo);
            }
        }
    }
}