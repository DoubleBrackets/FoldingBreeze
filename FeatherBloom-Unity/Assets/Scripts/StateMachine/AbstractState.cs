using UnityEngine;

namespace StateMachine
{
    public abstract class AbstractState : MonoBehaviour
    {
        public abstract bool CanReenter { get; protected set; }
        public abstract bool CanEnter { get; protected set; }

        public StateManager StateManager { get; set; }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnDeinitialize()
        {
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }
    }
}