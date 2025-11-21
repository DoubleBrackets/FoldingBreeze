using System;
using Events;
using Events.Core;
using Services;
using UnityEngine;

namespace Protag.Abilities
{
    public class FeatherResources : MonoBehaviour
    {
        [Serializable]
        public struct FeatherResourceState
        {
            public int CurrentFeathers;
            public int MaxFeathers;
        }

        [SerializeField]
        private float _featherReplenishedOnFan;

        [Header("Event Out")]

        [SerializeField]
        private FeatherResourceEvent _featherStateChangeEvent;

        [SerializeField]
        private VoidEvent _onFanSelfEvent;

        [Header("Config")]

        [SerializeField]
        private FeatherResourceState _initialState;

        [SerializeField]
        private TimeScaleService.TimeScaleEntryConfig _timeScaleOnHeal;

        private FeatherResourceState _currentState;

        public void Awake()
        {
            _currentState = _initialState;
        }

        private void Start()
        {
            _featherStateChangeEvent?.Raise(_currentState);
        }

        public void FanSelf()
        {
            if (_currentState.CurrentFeathers == _currentState.MaxFeathers)
            {
                return;
            }

            _currentState.CurrentFeathers = (int)Mathf.Min(_currentState.CurrentFeathers + _featherReplenishedOnFan,
                _currentState.MaxFeathers);

            _featherStateChangeEvent?.Raise(_currentState);
            _onFanSelfEvent?.Raise();

            TimeScaleService.Instance.NewTimeScaling(_timeScaleOnHeal);
        }

        public bool TryConsumeFeathers(int amount)
        {
            int current = _currentState.CurrentFeathers;

            int newFeathers = current - amount;

            if (newFeathers >= 0)
            {
                _currentState.CurrentFeathers = newFeathers;
                _featherStateChangeEvent?.Raise(_currentState);
                return true;
            }

            return false;
        }

        public bool HasFeathers()
        {
            return _currentState.CurrentFeathers > 0;
        }
    }
}