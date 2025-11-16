using Protag;
using UnityEngine;

namespace Utils
{
    /// <summary>
    ///     Test for looping pratag in environment
    /// </summary>
    public class ProtagLooper : MonoBehaviour
    {
        [SerializeField]
        private float _loopZPosition;

        private void Start()
        {
            Protaganist.Instance.SetPosition(transform.position);
        }

        private void Update()
        {
            if (Protaganist.Instance.Position.z > _loopZPosition)
            {
                Protaganist.Instance.SetPosition(transform.position);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.2f);

            Gizmos.color = Color.gray;
            Gizmos.DrawCube(new Vector3(0, 0, _loopZPosition), new Vector3(100, 100, 0.5f));
        }
    }
}