using Events;
using Protag.Abilities;
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
        private GroundChecker _groundChecker;

        [SerializeField]
        private ProtagCamera _protagCamera;

        [SerializeField]
        private ImpactSaver _impactSaver;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private GustAbility _gustAbility;

        [Header("States")]

        [SerializeField]
        private UpdraftState _updraftState;

        [SerializeField]
        private AbstractState _glideState;

        [Header("Config")]

        [SerializeField]
        private float _verticalImpactBoostRatio;

        [SerializeField]
        private Vector2 _verticalImpactBoostRange;

        [Header("Event Out")]

        [SerializeField]
        private VoidEvent _onStartSurf;

        [SerializeField]
        private VoidEvent _onStopSurf;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        private float _boost;
        private bool _surfing;

        public override void OnInitialize()
        {
            _impactSaver.OnTerrainImpact.AddListener(HandleTerrainImpact);
        }

        public override void OnDeinitialize()
        {
            _impactSaver.OnTerrainImpact.RemoveListener(HandleTerrainImpact);
        }

        private void HandleUpdraft()
        {
            if (Protaganist.IsFanOpen)
            {
                StateManager.SwitchState(_updraftState);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);
            Protaganist.OnTryUpdraft += HandleUpdraft;
            Protaganist.OnTryGust += HandleGust;
            _surfing = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);
            Protaganist.OnTryUpdraft -= HandleUpdraft;
            Protaganist.OnTryGust -= HandleGust;

            if (_surfing)
            {
                _onStopSurf.Raise();
                _surfing = false;
            }
        }

        private void HandleGust()
        {
            if (Protaganist.IsFanOpen)
            {
                _gustAbility.TryGust();
            }
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

            _animator.SetBool("IsGrounded", groundInfo.IsGrounded);
            _animator.SetBool("FanOpen", Protaganist.IsFanOpen);

            bool isSurfing = groundInfo.IsGrounded;

            if (!_surfing && isSurfing)
            {
                _onStartSurf.Raise();
            }
            else if (_surfing && !isSurfing)
            {
                _onStopSurf.Raise();
            }

            _surfing = isSurfing;

            if (!groundInfo.IsGrounded && Protaganist.IsFanOpen)
            {
                StateManager.SwitchState(_glideState);
            }
        }
    }
}