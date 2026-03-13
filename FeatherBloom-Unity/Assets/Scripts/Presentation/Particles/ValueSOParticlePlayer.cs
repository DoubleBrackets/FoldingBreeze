using System.Collections.Generic;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Presentation.Particles
{
    public class ValueSOParticlePlayer : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private List<ParticleSystem> _particleSystems = new();

        [SerializeField]
        private BoolValueSO _valueSO;

        [SerializeField]
        private bool _valueToPlayOn;

        private void Start()
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
                foreach (ParticleSystem ps in _particleSystems)
                {
                    ps.Play();
                }
            }
        }
    }
}