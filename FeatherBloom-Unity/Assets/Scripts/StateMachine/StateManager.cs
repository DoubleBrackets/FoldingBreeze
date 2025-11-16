using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class StateManager : MonoBehaviour
    {
        [Header("Config")]

        [SerializeField]
        private List<AbstractState> _states;

        [SerializeField]
        private AbstractState _initialState;

        private AbstractState _currentState;

        private bool _isInitialized = false;

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            foreach (AbstractState state in _states)
            {
                state.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
            {
                return;
            }

            foreach (AbstractState state in _states)
            {
                state.OnFixedUpdate();
            }
        }

        public void Deinitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            foreach (AbstractState state in _states)
            {
                state.OnDeinitialize();
            }
        }

        public void Initialize()
        {
            foreach (AbstractState state in _states)
            {
                state.OnInitialize();
                state.StateManager = this;
            }

            SwitchState(_initialState);
        }

        public void SwitchState(AbstractState state)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (state == _currentState)
            {
                if (!state.CanReenter)
                {
                    Debug.LogWarning($"StateMachine: Cannot re-enter state {state.name}");
                }
            }

            if (!state.CanEnter)
            {
                Debug.LogWarning($"StateMachine: Cannot enter state {state.name}");
                return;
            }

            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = state;
            _currentState.OnEnter();
        }
    }
}