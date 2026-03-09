using Protag.Gliding;
using Protag.Presentation;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    /// <summary>
    ///     State if protag is both airborn and fan is open
    /// </summary>
    public class ProtagDiveState : ProtagState
    {
        [SerializeField]
        private GlideMovement _glideMovement;

        [SerializeField]
        private GlideVisuals _glideVisuals;

        [SerializeField]
        private ProtagCamera _camera;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isDivingValueSO;

        [Header("Config")]

        [SerializeField]
        private GlideConfigSO _diveConfig;

        public override bool CanReenter => false;

        public override bool CanEnter => true;

        public override void OnInitialize()
        {
            base.OnInitialize();

            _isDivingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isDivingValueSO.SetValue(true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _isDivingValueSO.SetValue(false);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Vector2 aim = Protaganist.AimInput;
            aim.y = -1;
            float deltaTime = Time.fixedDeltaTime;

            _glideMovement.Tick(aim, _diveConfig, deltaTime);
            _glideVisuals.UpdateVisuals(aim, _glideMovement.CurrentVelocity, deltaTime);
            _camera.UpdateProtagCamera(aim.x, deltaTime, _glideMovement.CurrentVelocity);
        }
    }
}