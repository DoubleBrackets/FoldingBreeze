using Framework;
using Input.SerialComms;
using Protag.GestureHandlers;
using Protag.Gliding;
using Protag.Presentation;
using Protag.Updraft;
using StateMachine;
using UnityEngine;
using UnityEngine.Serialization;
using ValueSO.Core;

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

        [FormerlySerializedAs("_featherResources")]
        [SerializeField]
        private FeatherSystem.FeatherSystem _featherSystem;

        [SerializeField]
        private UpdraftState _updraftState;

        [SerializeField]
        private Animator _animator;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isGlidingValueSO;

        [SerializeField]
        private BoolValueSO _isFallingWithWingsValueSO;

        [Header("Config")]

        [SerializeField]
        private GlideConfigSO _glideConfig;

        [SerializeField]
        private UpdraftConfigSO _updraftConfig;

        private BoxFanArduinoComm _boxFanArduinoComm;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _boxFanArduinoComm = ServiceLocator.GetService<BoxFanArduinoComm>();

            _isGlidingValueSO.SetValue(false);
            _isFallingWithWingsValueSO.SetValue(false);
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
            if (Protaganist.IsFanOpen && _featherSystem.TryConsumeFeathers(_updraftConfig.FeathersConsumed))
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

            _isGlidingValueSO.SetValue(false);
            _isFallingWithWingsValueSO.SetValue(false);
        }

        private void HandleBoost(float amount)
        {
            _glideMovement.Boost(amount);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            bool canGlide =
                _featherSystem.TryConsumeFeathers(_glideConfig.FeatherConsumptionPerSecond * Time.fixedDeltaTime);

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();

            Vector2 aim = Protaganist.AimInput;
            float deltaTime = Time.fixedDeltaTime;

            if (canGlide)
            {
                _glideMovement.Tick(aim, _glideConfig, deltaTime);
            }
            else
            {
                _glideMovement.Tick(Vector2.down, _glideConfig, deltaTime);
            }

            _isGlidingValueSO.SetValue(canGlide);
            _isFallingWithWingsValueSO.SetValue(!canGlide);

            _glideVisuals.UpdateVisuals(aim, _glideMovement.CurrentVelocity, deltaTime);

            _camera.UpdateProtagCamera(aim.x, deltaTime, _glideMovement.CurrentVelocity);

            _animator.SetBool("IsGrounded", groundInfo.IsGrounded);
            _animator.SetBool("FanOpen", Protaganist.IsFanOpen);

            if (groundInfo.IsGrounded || !Protaganist.IsFanOpen)
            {
                StateManager.SwitchState(_surfState);
            }
        }
    }
}