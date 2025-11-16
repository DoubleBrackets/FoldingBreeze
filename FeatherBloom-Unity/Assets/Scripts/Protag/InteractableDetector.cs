using UnityEngine;
using UnityEngine.Events;

namespace Protag
{
    public class InteractableDetector : MonoBehaviour
    {
        public UnityEvent<float> OnBoostPickup;
        public UnityEvent OnTouchHazard;

        public void PickupBoost(float boostAmount)
        {
            OnBoostPickup?.Invoke(boostAmount);
        }

        public void TouchHazard()
        {
            Debug.Log("Hazard Touched");
            OnTouchHazard?.Invoke();
        }
    }
}