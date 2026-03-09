using FMODUnity;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Presentation.SFX
{
    public class ValueSOSfxPlayer : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private StudioEventEmitter _sfxEmitter;

        [SerializeField]
        private BoolValueSO _valueSO;

        [SerializeField]
        private bool _valueToPlayOn;

        private void Awake()
        {
            _valueSO.AddListener(this, OnValueChange);
        }

        private void OnDestroy()
        {
            _valueSO.RemoveListener(this);
        }

        private void OnValueChange(bool newValue)
        {
            if (newValue == _valueToPlayOn)
            {
                _sfxEmitter.Play();
            }
        }
    }
}