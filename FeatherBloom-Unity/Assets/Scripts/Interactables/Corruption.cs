using Cysharp.Threading.Tasks;
using DevTools;
using Protag;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    /// <summary>
    ///     Interactable corruption that can be blown away
    /// </summary>
    public class Corruption : MonoBehaviour
    {
        [SerializeField]
        private float _cleanupDelay;

        [SerializeField]
        public UnityEvent _onBlowAway;

        [SerializeField]
        private UnityEvent _onCleanup;

        public Vector3 Position => transform.position;

        public bool IsAlreadyTargeted { get; private set; }

        private bool _blownAway;

        public void BlowAway()
        {
            if (_blownAway)
            {
                return;
            }

            _blownAway = true;
            _onBlowAway?.Invoke();

            HandleBlownAway().Forget();
        }

        private async UniTaskVoid HandleBlownAway()
        {
            await UniTask.WaitForSeconds(_cleanupDelay);
            _onCleanup?.Invoke();
        }

        public void MarkAsTargeted()
        {
            IsAlreadyTargeted = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_blownAway)
            {
                return;
            }

            var interactor = other.GetComponentInParent<InteractableDetector>();
            if (interactor != null)
            {
                interactor.TouchHazard();
                BlowAway();
            }
        }


        private void OnDrawGizmos()
        {
            LabelUtils.Label(transform.position, "Hazard");
        }
    }
}