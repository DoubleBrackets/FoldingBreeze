using UnityEngine;
using UnityEngine.Events;

namespace Protag
{
    public class InteractableDetector : MonoBehaviour
    {
        public UnityEvent<float> OnBoostPickup;

        public void PickupBoost(float boostAmount)
        {
            OnBoostPickup?.Invoke(boostAmount);
        }
    }
}