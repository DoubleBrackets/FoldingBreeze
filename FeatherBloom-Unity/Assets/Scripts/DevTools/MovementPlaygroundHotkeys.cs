using System.Collections.Generic;
using Protag;
using UnityEngine;

namespace DevTools
{
    public class MovementPlaygroundHotkeys : MonoBehaviour
    {
        [SerializeField]
        private Protaganist _protag;

        [SerializeField]
        private List<Transform> _spawnPoints;

        private void Update()
        {
            // Convert numkey press to index
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (Transform spawnPoint in _spawnPoints)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.forward * 5f);
                LabelUtils.Label(spawnPoint.position, spawnPoint.name);
            }
        }
    }
}