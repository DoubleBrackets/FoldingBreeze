using Protag.Surfing;
using StateMachine;
using UnityEngine;

namespace Protag.States
{
    public class ProtagSurfState : ProtagState
    {
        [SerializeField]
        private SurfMovement _surfMovement;

        [SerializeField]
        private SurfVisuals _surfVisuals;

        [SerializeField]
        private AbstractState _glideState;

        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private ProtagCamera _protagCamera;

        [SerializeField]
        private ImpactSaver _impactSaver;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [Header("Config")]

        [SerializeField]
        private float _verticalImpactBoostRatio;

        [SerializeField]
        private Vector2 _verticalImpactBoostRange;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        private float _boost;

        public override void OnInitialize()
        {
            _impactSaver.OnTerrainImpact.AddListener(HandleTerrainImpact);
        }

        public override void OnDeinitialize()
        {
            _impactSaver.OnTerrainImpact.RemoveListener(HandleTerrainImpact);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);
        }

        private void HandleBoostPickup(float boostAmount)
        {
            _boost += boostAmount;
        }

        private void HandleTerrainImpact(ImpactSaver.ImpactInfo info)
        {
            // Save vertical impulse as boost
            Vector3 impulse = info.Impulse;
            _boost = Mathf.Max(0, impulse.y) * _verticalImpactBoostRatio;

            if (_boost < _verticalImpactBoostRange.x)
            {
                _boost = 0;
            }
            else if (_boost > _verticalImpactBoostRange.y)
            {
                _boost = _verticalImpactBoostRange.y;
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            float deltaTime = Time.fixedDeltaTime;

            // No steering when fan is open
            float horizontalInput = Protaganist.IsFanOpen ? 0 : Protaganist.AimInput.x;

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();
            _surfMovement.Tick(horizontalInput, groundInfo, _boost, deltaTime);
            _boost = 0;
            _surfVisuals.UpdateSurfVisuals(groundInfo, _surfMovement.CurrentVelocity, horizontalInput, deltaTime);

            _protagCamera.UpdateProtagCamera(
                horizontalInput,
                deltaTime,
                _surfMovement.CurrentVelocity);

            if (!groundInfo.IsGrounded && Protaganist.IsFanOpen)
            {
                StateManager.SwitchState(_glideState);
            }
        }
    }
}