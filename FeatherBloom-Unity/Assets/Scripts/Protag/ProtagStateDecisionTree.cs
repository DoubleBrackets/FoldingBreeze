using Protag.Movement;
using Protag.States;
using UnityEngine;

namespace Protag
{
    /// <summary>
    ///     Decision tree to determine protag state
    /// </summary>
    public class ProtagStateDecisionTree : MonoBehaviour
    {
        [Header("Fixed States")]

        [SerializeField]
        private UpdraftState _updraftState;

        [SerializeField]
        private ProtagState _deadState;

        [SerializeField]
        private HealingState _healingState;

        [Header("Free Movement States")]

        [SerializeField]
        private ProtagState _dashSurfState;

        [SerializeField]
        private ProtagState _surfWingState;

        [SerializeField]
        private ProtagState _glideState;

        [SerializeField]
        private ProtagState _diveState;

        [SerializeField]
        private ProtagState _tumbleState;

        public ProtagState EvaluateNewState(ProtagState currentState,
            GroundChecker.GroundedInfo groundedInfo,
            bool isFanOpen,
            bool tryUpdraft = false,
            bool tryHealing = false,
            bool shouldDie = false)
        {
            if (shouldDie)
            {
                return _deadState;
            }

            if (currentState == null)
            {
                Debug.LogError("Current state is null!");
                return currentState;
            }

            // One-shots
            if (currentState == _deadState)
            {
                return _deadState;
            }

            bool canEnterHealing = _healingState.CanEnter;
            bool validHealingEnterState = currentState == _surfWingState || currentState == _glideState;

            if (tryHealing && canEnterHealing && validHealingEnterState)
            {
                return _healingState;
            }

            if (currentState == _healingState && !_healingState.IsFinished)
            {
                return _healingState;
            }

            bool canEnterUpdraft = _updraftState.CanEnter;
            bool validUpdraftEnterState = currentState == _surfWingState || currentState == _glideState;
            if (tryUpdraft && canEnterUpdraft && validUpdraftEnterState)
            {
                return _updraftState;
            }

            if (currentState == _updraftState && !_updraftState.IsFinished)
            {
                return _updraftState;
            }

            // Grounded
            if (groundedInfo.IsGrounded)
            {
                if (isFanOpen)
                {
                    return _surfWingState;
                }

                return _dashSurfState;
            }

            // Airborne
            if (isFanOpen)
            {
                if (_glideState.CanEnter)
                {
                    return _glideState;
                }

                return _tumbleState;
            }

            return _diveState;
        }

        public bool IsInOneshotState(ProtagState state)
        {
            return state == _updraftState || state == _healingState;
        }
    }
}