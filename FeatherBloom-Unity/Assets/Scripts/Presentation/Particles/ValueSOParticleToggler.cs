using System.Collections.Generic;
using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Presentation.Particles
{
    /// <summary>
    ///     Toggles a particle system based on a ValueSO
    /// </summary>
    public class ValueSOParticleToggler : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private List<ParticleSystem> _particleSystems = new();

        [SerializeField]
        private BoolValueSO _valueSO;

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
            if (newValue)
            {
                foreach (ParticleSystem ps in _particleSystems)
                {
                    ps.Play();
                }
            }
            else
            {
                foreach (ParticleSystem ps in _particleSystems)
                {
                    ps.Stop();
                }
            }
        }
    }
}