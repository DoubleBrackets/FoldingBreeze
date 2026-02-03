using StateMachine;
using UnityEngine;
using UnityEngine.Events;

namespace Protag
{
    public abstract class ProtagState : AbstractState
    {
        [Header("Protag State Dependencies")]

        [SerializeField]
        protected Protaganist Protaganist;

        [Header("Protag State Events")]

        [SerializeField]
        private UnityEvent _onEnterState;

        [SerializeField]
        private UnityEvent _onExitState;

        public override void OnEnter()
        {
            base.OnEnter();
            _onEnterState?.Invoke();
        }

        public override void OnExit()
        {
            base.OnExit();
            _onExitState?.Invoke();
        }
    }
}