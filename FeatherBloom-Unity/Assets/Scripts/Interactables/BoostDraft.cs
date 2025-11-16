using Protag;
using UnityEngine;

namespace Interactables
{
    public class BoostDraft : MonoBehaviour
    {
        [SerializeField]
        private float _boostAmount;

        private void OnTriggerEnter(Collider other)
        {
            var interactableDetector = other.GetComponentInParent<InteractableDetector>();
            if (interactableDetector != null)
            {
                interactableDetector.PickupBoost(_boostAmount);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
        }
    }
}