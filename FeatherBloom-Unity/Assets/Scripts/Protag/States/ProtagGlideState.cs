using Protag.Gliding;
using SerialComms;
using StateMachine;
using UnityEngine;

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

        [SerializeField]
        private Animator _animator;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoost);
            BoxFanArduinoComm.Instance?.WriteFanOn(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoost);
            BoxFanArduinoComm.Instance?.WriteFanOn(false);
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

            _animator.SetBool("IsGrounded", groundInfo.IsGrounded);
            _animator.SetBool("FanOpen", Protaganist.IsFanOpen);

            if (groundInfo.IsGrounded || !Protaganist.Instance.IsFanOpen)
            {
                StateManager.SwitchState(_surfState);
            }
        }
    }
}