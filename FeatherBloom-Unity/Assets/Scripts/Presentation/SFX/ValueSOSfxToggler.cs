using FMODUnity;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Presentation.SFX
{
    /// <summary>
    ///     Toggles a particle system based on a ValueSO
    /// </summary>
    public class ValueSOSfxToggler : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private StudioEventEmitter _sfxEmitter;

        [SerializeField]
        private BoolValueSO _valueSO;

        private void Start()
        {
            _valueSO.AddListener(this, OnValueChange, true);
        }

        private void OnDestroy()
        {
            _valueSO.RemoveListener(this);
        }

        private void OnValueChange(bool newValue)
        {
            if (newValue)
            {
                _sfxEmitter.Play();
            }
            else
            {
                _sfxEmitter.Stop();
            }
        }
    }
}