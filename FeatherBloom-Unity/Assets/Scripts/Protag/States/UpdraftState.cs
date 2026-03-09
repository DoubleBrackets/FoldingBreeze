using Framework;
using Input.SerialComms;
using Protag.FeatherSystem;
using Protag.Movement;
using Protag.Updraft;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    public class UpdraftState : ProtagState
    {
        [Header("Depends")]

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [SerializeField]
        private FeatherManager _featherManager;

        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private Rigidbody _rb;

        [Header("Config")]

        [SerializeField]
        private UpdraftConfigSO _updraftConfig;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isUpdraftingValueSO;

        public override bool CanReenter => false;
        public override bool CanEnter => _featherManager.FeatherValue > _updraftConfig.FeathersConsumed;

        private float _stateTimer;

        public bool IsFinished => _stateTimer <= 0f;

        private Vector3 _entryHorizontalVelocity;
        private Vector3 _launchNormal;

        private BoxFanArduinoComm _boxFanArduinoComm;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _boxFanArduinoComm = ServiceLocator.GetService<BoxFanArduinoComm>();

            _isUpdraftingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _groundChecker.ForceUnground(0.1f);

            _launchNormal = Vector3.up;
            _entryHorizontalVelocity = Vector3.ProjectOnPlane(_rb.linearVelocity, _launchNormal)
                                       * _updraftConfig.HorizontalVelocityKeepRatio;

            UpdateVelocity(_launchNormal);

            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);
            _boxFanArduinoComm?.WriteFanOn(true);
            _isUpdraftingValueSO.SetValue(true);
            _stateTimer = _updraftConfig.Duration;
        }

        public override void OnExit()
        {
            base.OnExit();

            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);
            _boxFanArduinoComm?.WriteFanOn(false);

            _isUpdraftingValueSO.SetValue(false);
        }

        private void HandleBoostPickup(float boost)
        {
            _entryHorizontalVelocity =
                _entryHorizontalVelocity.normalized * (_entryHorizontalVelocity.magnitude + boost);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _stateTimer -= Time.fixedDeltaTime;
            UpdateVelocity(_launchNormal);
        }

        private void UpdateVelocity(Vector3 normal)
        {
            float t = 1 - _stateTimer / _updraftConfig.Duration;
            float ratio = Mathf.Clamp01(_updraftConfig.UpdraftVelocityCurve.Evaluate(t));
            _rb.linearVelocity = normal * _updraftConfig.UpdraftVelocity * ratio + _entryHorizontalVelocity;
        }
    }
}