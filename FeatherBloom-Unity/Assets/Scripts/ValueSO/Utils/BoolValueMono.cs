using UnityEngine;
using UnityEngine.Events;
using ValueSO.Core;

namespace ValueSO.Utils
{
    public class BoolValueMono : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private BoolValueSO _valueSO;

        [SerializeField]
        private bool _invert;

        [SerializeField]
        private UnityEvent<bool> _onValueChange;

        private void Awake()
        {
            _valueSO.AddListener(this, OnValueChange, true);
        }

        private void OnDestroy()
        {
            _valueSO.RemoveListener(this);
        }

        private void OnValueChange(bool newValue)
        {
            bool value = _invert ? !newValue : newValue;
            _onValueChange?.Invoke(value);
        }
    }
}