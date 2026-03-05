using Framework;
using Input.SerialComms;
using Protag.Abilities;
using Protag.GestureHandlers;
using Protag.Gliding;
using StateMachine;
using UnityEngine;

namespace Protag.States
{
    /// <summary>
    ///     State if protag is both airborn and fan is open
    /// </summary>
    public class ProtagGlideState : ProtagState, IUpdraftHandler
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

        [SerializeField]
        private FeatherResources _featherResources;

        [SerializeField]
        private UpdraftState _updraftState;

        [SerializeField]
        private Animator _animator;

        private BoxFanArduinoComm _boxFanArduinoComm;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _boxFanArduinoComm = ServiceLocator.GetService<BoxFanArduinoComm>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoost);
            _boxFanArduinoComm?.WriteFanOn(true);
            Protaganist.RegisterUpdraftHandler(this);
        }

        public GestureHandleResult HandleUpdraft()
        {
            if (Protaganist.IsFanOpen && _featherResources.TryConsumeFeathers(1))
            {
                StateManager.SwitchState(_updraftState);
                return new GestureHandleResult
                    { DidSucceed = true, TimeslowdownBlockDuration = _updraftState.TimeSlowdownBlockDuration };
            }

            return new GestureHandleResult { DidSucceed = false };
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoost);

            _boxFanArduinoComm?.WriteFanOn(false);
        }

        private void HandleBoost(float amount)
        {
            _glideMovement.Boost(amount);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();

            float horizontalInput = Protaganist.AimInput.x;
            float verticalInput = Protaganist.AimInput.y;
            float deltaTime = Time.fixedDeltaTime;

            _glideMovement.Tick(horizontalInput, verticalInput, deltaTime);
            _glideVisuals.UpdateVisuals(horizontalInput, verticalInput, _glideMovement.CurrentVelocity, deltaTime);

            _camera.UpdateProtagCamera(horizontalInput, deltaTime, _glideMovement.CurrentVelocity);

            _animator.SetBool("IsGrounded", groundInfo.IsGrounded);
            _animator.SetBool("FanOpen", Protaganist.IsFanOpen);

            if (groundInfo.IsGrounded || !Protaganist.IsFanOpen)
            {
                StateManager.SwitchState(_surfState);
            }
        }
    }
}