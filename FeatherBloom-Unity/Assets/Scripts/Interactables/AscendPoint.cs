using DevTools;
using Protag;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    /// <summary>
    ///     Interactable that completes game on touched
    /// </summary>
    public class AscendPoint : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _onAscend;

        private bool _ascended;

        private void OnTriggerEnter(Collider other)
        {
            if (_ascended)
            {
                return;
            }

            var interactor = other.GetComponentInParent<InteractableDetector>();
            if (interactor != null)
            {
                interactor.TouchEnding();
                _ascended = true;
                _onAscend?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            LabelUtils.Label(transform.position, "Game End");
        }
    }
}