using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/Basic/VoidEvent")]
    public class VoidEvent : SOEvent
    {
        public void Raise()
        {
            if (_debug)
            {
                Debug.Log($"Event {name} raised");
            }

            _internalVoidEvent?.Invoke();
        }
    }
}