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

        public AbstractState CurrentState => _currentState;

        private AbstractState _currentState;

        private bool _isInitialized;

        public void UpdateStateMachine()
        {
            if (!_isInitialized)
            {
                return;
            }

            _currentState.OnUpdate();
        }

        public void FixedUpdateStateMachine()
        {
            if (!_isInitialized)
            {
                return;
            }

            _currentState.OnFixedUpdate();
        }

        public void Deinitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            _currentState.OnExit();

            foreach (AbstractState state in _states)
            {
                state.OnDeinitialize();
            }
        }

        public void Initialize()
        {
            _isInitialized = true;
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
                    return;
                }
            }

            if (!state.CanEnter)
            {
                return;
            }

            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = state;
            _currentState.OnEnter();
        }

        [ContextMenu("Autodetect children")]
        private void AutoDetectChildren()
        {
            _states = new List<AbstractState>(GetComponentsInChildren<AbstractState>());
        }
    }
}