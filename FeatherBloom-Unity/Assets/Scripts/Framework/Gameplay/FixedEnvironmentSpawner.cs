using UnityEngine;

namespace Framework.Gameplay
{
    /// <summary>
    ///     Handles generating the pillar n stuff
    /// </summary>
    public class FixedEnvironmentSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pillarSectionPrefab;

        [SerializeField]
        private Transform _pillarCollider;

        [SerializeField]
        private float _pillarSectionHeight;

        [SerializeField]
        private Transform _pillarRoot;

        [SerializeField]
        private GameObject _pillarCapPrefab;

        public void SpawnPillar(float desiredHeight)
        {
            _pillarCollider.localScale = new Vector3(1f, desiredHeight * 2f, 1f);
            int sectionCount = Mathf.CeilToInt(desiredHeight / _pillarSectionHeight);
            for (var i = 0; i < sectionCount; i++)
            {
                GameObject section = Instantiate(_pillarSectionPrefab, _pillarRoot);
                section.transform.localPosition = Vector3.up * i * _pillarSectionHeight;
            }

            GameObject cap = Instantiate(_pillarCapPrefab, _pillarRoot);
            cap.transform.localPosition = Vector3.up * desiredHeight;
        }
    }
}