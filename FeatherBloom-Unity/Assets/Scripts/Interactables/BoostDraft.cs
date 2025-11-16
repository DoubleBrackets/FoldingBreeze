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
    }
}