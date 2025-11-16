using Protag;
using UnityEngine;

namespace Utils
{
    /// <summary>
    ///     Test for looping pratag in environment
    /// </summary>
    public class ProtagRespawner : MonoBehaviour
    {
        [SerializeField]
        private float _respawnYPosition;

        private void Start()
        {
            Protaganist.Instance.SetPositionAndDirection(transform.position, transform.forward);
        }

        private void Update()
        {
            if (Protaganist.Instance.Position.y < _respawnYPosition)
            {
                Protaganist.Instance.SetPositionAndDirection(transform.position, transform.forward);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10f);

            Gizmos.color = Color.gray;
            Gizmos.DrawCube(new Vector3(0, _respawnYPosition, 0), new Vector3(100, 0.5f, 100));
        }
    }
}