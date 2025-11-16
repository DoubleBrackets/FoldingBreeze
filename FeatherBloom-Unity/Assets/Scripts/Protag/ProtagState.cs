using StateMachine;
using UnityEngine;

namespace Protag
{
    public abstract class ProtagState : AbstractState
    {
        [Header("Protag State Dependencies")]

        [SerializeField]
        protected Protaganist Protaganist;
    }
}