using UnityEngine;
using ValueSO.Core;

namespace Utils
{
    /// <summary>
    ///     Syncs height of a transform to a ValueSO
    /// </summary>
    public class HeightSyncer : MonoBehaviour
    {
        [Header("ValueSO (Read)")]

        [SerializeField]
        private FloatValueSO _targetHeight;

        [SerializeField]
        private Transform _targetTransform;

        private void Update()
        {
            _targetTransform.position = new Vector3(_targetTransform.position.x, _targetHeight.Value,
                _targetTransform.position.z);
        }
    }
}