using DebugTools;
using Protag;
using UnityEngine;

namespace Interactables
{
    public class StageHazard : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var interactor = other.GetComponentInParent<InteractableDetector>();
            if (interactor != null)
            {
                interactor.TouchHazard();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            var interactor = other.collider.GetComponentInParent<InteractableDetector>();
            if (interactor != null)
            {
                interactor.TouchHazard();
            }
        }

        private void OnDrawGizmos()
        {
            LabelUtils.Label(transform.position, "Hazard");
        }
    }
}