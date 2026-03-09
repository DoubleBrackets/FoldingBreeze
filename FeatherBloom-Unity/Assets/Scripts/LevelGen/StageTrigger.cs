using UnityEngine;
using UnityEngine.Events;

namespace Protag.LevelGen
{
    /// <summary>
    ///     Handles detecting when the Protag enters a new stage section.
    /// </summary>
    public class StageTrigger : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent OnProtagEnterSection;

        [SerializeField]
        public UnityEvent OnProtagExitSection;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<InteractableDetector>() != null)
            {
                OnProtagEnterSection?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<InteractableDetector>() != null)
            {
                OnProtagExitSection?.Invoke();
            }
        }
    }
}