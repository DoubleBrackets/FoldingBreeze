using UnityEngine;
using UnityEngine.Serialization;

public class ProjectTest : MonoBehaviour
{
    [FormerlySerializedAs("mask")]
    [SerializeField]
    private LayerMask _mask;

    private void OnDrawGizmos()
    {
        bool hit = Physics.Raycast(transform.position, Vector2.down, out RaycastHit raycastHit, Mathf.Infinity, _mask);

        Vector3 normal = raycastHit.normal;
        Vector3 forward = transform.forward;

        Vector3 projected = Vector3.ProjectOnPlane(forward, normal);

        Vector3 flattened = new Vector3(projected.x, 0, projected.z).normalized;

        var dist = 10;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + forward * dist);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + projected * dist);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + flattened * dist);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + normal * dist);
    }
}