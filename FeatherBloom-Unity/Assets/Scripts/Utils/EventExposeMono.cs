using Events;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class EventExposeMono : MonoBehaviour
    {
        [SerializeField]
        private SOEvent _eventSO;

        [SerializeField]
        private UnityEvent _onRaiseEvent;

        private void Awake()
        {
            _eventSO.AddListener(HandleEventSO);
        }

        private void OnDestroy()
        {
            _eventSO.RemoveListener(HandleEventSO);
        }

        private void HandleEventSO()
        {
            _onRaiseEvent?.Invoke();
        }
    }
}