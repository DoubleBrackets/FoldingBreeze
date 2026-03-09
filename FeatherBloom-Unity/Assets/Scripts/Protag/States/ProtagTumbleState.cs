using Protag.Gliding;
using Protag.Presentation;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    /// <summary>
    ///     Tumbling state
    /// </summary>
    public class ProtagTumbleState : ProtagState
    {
        [SerializeField]
        private GlideMovement _glideMovement;

        [SerializeField]
        private GlideVisuals _glideVisuals;

        [SerializeField]
        private ProtagCamera _camera;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isTumblingValueSO;

        [Header("Config")]

        [SerializeField]
        private GlideConfigSO _tumbleConfig;

        public override bool CanReenter => false;

        public override bool CanEnter => true;

        public override void OnInitialize()
        {
            base.OnInitialize();

            _isTumblingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _isTumblingValueSO.SetValue(true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _isTumblingValueSO.SetValue(false);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            Vector2 aim = Vector2.down;
            float deltaTime = Time.fixedDeltaTime;

            _glideMovement.Tick(aim, _tumbleConfig, deltaTime);
            _glideVisuals.UpdateVisuals(aim, _glideMovement.CurrentVelocity, deltaTime);
            _camera.UpdateProtagCamera(aim.x, deltaTime, _glideMovement.CurrentVelocity);
        }
    }
}