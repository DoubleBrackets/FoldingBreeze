using Cysharp.Threading.Tasks;
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
    }
}