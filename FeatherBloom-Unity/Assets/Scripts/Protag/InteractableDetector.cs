using UnityEngine;
using UnityEngine.Events;

namespace Protag
{
    public class InteractableDetector : MonoBehaviour
    {
        public UnityEvent<Vector3> OnBoostPickup;
        public UnityEvent OnTouchHazard;
        public UnityEvent OnTouchEnding;

        public void PickupBoost(Vector3 boost)
        {
            OnBoostPickup?.Invoke(boost);
        }

        public void TouchHazard()
        {
            Debug.Log("Hazard Touched");
            OnTouchHazard?.Invoke();
        }

        public void TouchEnding()
        {
            Debug.Log("Ending Touched");
            OnTouchEnding?.Invoke();
        }
    }
}