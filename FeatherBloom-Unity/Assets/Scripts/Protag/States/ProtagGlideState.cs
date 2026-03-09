using Framework;
using Input.SerialComms;
using Protag.FeatherSystem;
using Protag.Gliding;
using Protag.Presentation;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    /// <summary>
    ///     Gliding state
    /// </summary>
    public class ProtagGlideState : ProtagState
    {
        [SerializeField]
        private GlideMovement _glideMovement;

        [SerializeField]
        private GlideVisuals _glideVisuals;

        [SerializeField]
        private ProtagCamera _camera;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [SerializeField]
        private FeatherManager _featherManager;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isGlidingValueSO;

        [Header("Config")]

        [SerializeField]
        private GlideConfigSO _glideConfig;

        private BoxFanArduinoComm _boxFanArduinoComm;

        public override bool CanReenter => false;

        public override bool CanEnter => _featherManager.FeatherValue > FeatherConsumedPerFixedUpdate;

        private float FeatherConsumedPerFixedUpdate => _glideConfig.FeatherConsumptionPerSecond * Time.fixedDeltaTime;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _boxFanArduinoComm = ServiceLocator.GetService<BoxFanArduinoComm>();

            _isGlidingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoost);
            _boxFanArduinoComm?.WriteFanOn(true);

            _isGlidingValueSO.SetValue(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoost);
            _boxFanArduinoComm?.WriteFanOn(false);

            _isGlidingValueSO.SetValue(false);
        }

        private void HandleBoost(float amount)
        {
            _glideMovement.Boost(amount);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            _featherManager.TryConsumeFeathers(_glideConfig.FeatherConsumptionPerSecond * Time.fixedDeltaTime);

            Vector2 aim = Protaganist.AimInput;
            float deltaTime = Time.fixedDeltaTime;

            _glideMovement.Tick(aim, _glideConfig, deltaTime);
            _glideVisuals.UpdateVisuals(aim, _glideMovement.CurrentVelocity, deltaTime);
            _camera.UpdateProtagCamera(aim.x, deltaTime, _glideMovement.CurrentVelocity);
        }
    }
}