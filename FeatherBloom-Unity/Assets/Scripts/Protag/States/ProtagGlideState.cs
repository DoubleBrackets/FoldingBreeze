using Protag.Gliding;
using StateMachine;
using UnityEngine;
using UnityEngine.Events;

namespace Protag.States
{
    /// <summary>
    ///     State if protag is both airborn and fan is open
    /// </summary>
    public class ProtagGlideState : ProtagState
    {
        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private GlideMovement _glideMovement;

        [SerializeField]
        private GlideVisuals _glideVisuals;

        [SerializeField]
        private AbstractState _surfState;

        [SerializeField]
        private ProtagCamera _camera;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [Header("Unity Events")]

        [SerializeField]
        private UnityEvent _onEnterGlide;

        [SerializeField]
        private UnityEvent _onExitGlide;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public override void OnEnter()
        {
            base.OnEnter();
            _onEnterGlide?.Invoke();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoost);
        }

        public override void OnExit()
        {
            base.OnExit();
            _onExitGlide?.Invoke();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoost);
        }

        private void HandleBoost(float amount)
        {
            _glideMovement.Boost(amount);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();

            float horizontalInput = Protaganist.Instance.AimInput.x;
            float verticalInput = Protaganist.Instance.AimInput.y;
            float deltaTime = Time.fixedDeltaTime;

            _glideMovement.Tick(horizontalInput, verticalInput, deltaTime);
            _glideVisuals.UpdateVisuals(horizontalInput, verticalInput, _glideMovement.CurrentVelocity, deltaTime);

            _camera.UpdateProtagCamera(horizontalInput, deltaTime, _glideMovement.CurrentVelocity);

            if (groundInfo.IsGrounded || !Protaganist.Instance.IsFanOpen)
            {
                StateManager.SwitchState(_surfState);
            }
        }
    }
}