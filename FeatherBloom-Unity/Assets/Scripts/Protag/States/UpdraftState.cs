using Framework;
using Framework.Timescaling;
using Input.SerialComms;
using Protag.Surfing;
using Protag.Updraft;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    public class UpdraftState : ProtagState
    {
        [Header("Depends")]

        [SerializeField]
        private SurfMovement _surfMovement;

        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [Header("Config")]

        [SerializeField]
        private UpdraftConfigSO _updraftConfig;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isUpdraftingValueSO;

        [Header("Stateout")]

        [SerializeField]
        private ProtagState _airState;

        [SerializeField]
        private ProtagState _groundState;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public float TimeSlowdownBlockDuration => _updraftConfig.TimeSlowdownBlockDuration;

        private float _stateTimer;

        private Vector3 _horizontalVelocity;
        private Vector3 _launchNormal;

        private TimeScaleService _timeScaleService;
        private BoxFanArduinoComm _boxFanArduinoComm;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _timeScaleService = ServiceLocator.GetService<TimeScaleService>();
            _boxFanArduinoComm = ServiceLocator.GetService<BoxFanArduinoComm>();

            _isUpdraftingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _groundChecker.ForceUnground(0.1f);
            _stateTimer = _updraftConfig.Duration;
            _animator.SetBool("Updraft", true);

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();
            _launchNormal = groundInfo.GroundNormal;

            _horizontalVelocity = Vector3.ProjectOnPlane(_surfMovement.CurrentVelocity, _launchNormal)
                                  * _updraftConfig.HorizontalVelocityKeepRatio;

            UpdateVelocity(_launchNormal);

            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);

            _boxFanArduinoComm?.WriteFanOn(true);

            _isUpdraftingValueSO.SetValue(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _animator.SetBool("Updraft", false);

            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);
            _boxFanArduinoComm?.WriteFanOn(false);

            _isUpdraftingValueSO.SetValue(false);
        }

        private void HandleBoostPickup(float boost)
        {
            _horizontalVelocity = _horizontalVelocity.normalized * (_horizontalVelocity.magnitude + boost);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _stateTimer -= Time.fixedDeltaTime;

            if (_stateTimer < 0f)
            {
                GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();
                if (groundInfo.IsGrounded)
                {
                    StateManager.SwitchState(_groundState);
                }
                else
                {
                    StateManager.SwitchState(_airState);
                }
            }
            else
            {
                UpdateVelocity(_launchNormal);
            }
        }

        private void UpdateVelocity(Vector3 normal)
        {
            float t = 1 - _stateTimer / _updraftConfig.Duration;
            float ratio = Mathf.Clamp01(_updraftConfig.UpdraftVelocityCurve.Evaluate(t));
            _surfMovement.SetVelocity(normal * _updraftConfig.UpdraftVelocity * ratio + _horizontalVelocity);
        }
    }
}