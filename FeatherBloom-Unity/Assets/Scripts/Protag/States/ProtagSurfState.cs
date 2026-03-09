using Events;
using Protag.GestureHandlers;
using Protag.Gust;
using Protag.Presentation;
using Protag.Surfing;
using Protag.Updraft;
using StateMachine;
using UnityEngine;

namespace Protag.States
{
    public class ProtagSurfState : ProtagState, IUpdraftHandler, IBreezeHandler
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

        [SerializeField]
        private FeatherSystem.FeatherSystem _featherSystem;

        [Header("States")]

        [SerializeField]
        private UpdraftState _updraftState;

        [SerializeField]
        private AbstractState _glideState;

        [Header("Config")]

        [SerializeField]
        private SurfConfigSO _surfConfig;

        [SerializeField]
        private UpdraftConfigSO _updraftConfig;

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

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);
            Protaganist.RegisterUpdraftHandler(this);
            Protaganist.RegisterFanSelfHandler(this);

            _surfing = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);

            if (_surfing)
            {
                _onStopSurf.Raise();
                _surfing = false;
            }
        }

        public GestureHandleResult HandleBreeze()
        {
            // if (Protaganist.IsFanOpen)
            // {
            //     bool didFanSelf = _featherSystem.FanSelf();
            //     return new GestureHandleResult
            //     {
            //         DidSucceed = didFanSelf, TimeslowdownBlockDuration = _featherSystem.TimeSlowdownBlockDuration
            //     };
            // }

            return new GestureHandleResult { DidSucceed = false, TimeslowdownBlockDuration = 0f };
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

        private void HandleBoostPickup(float boostAmount)
        {
            _boost += boostAmount;
        }

        private void HandleTerrainImpact(ImpactSaver.ImpactInfo info)
        {
            float verticalBoostImpactRatio = _surfConfig.VerticalImpactBoostRatio;
            Vector2 verticalBoostRange = _surfConfig.VerticalImpactBoostRange;

            // Save vertical impulse as boost
            Vector3 impulse = info.Impulse;
            _boost = Mathf.Max(0, impulse.y) * verticalBoostImpactRatio;

            if (_boost < verticalBoostRange.x)
            {
                _boost = 0;
            }
            else if (_boost > verticalBoostRange.y)
            {
                _boost = verticalBoostRange.y;
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            float deltaTime = Time.fixedDeltaTime;

            // No steering when fan is open
            float horizontalInput = Protaganist.IsFanOpen ? 0 : Protaganist.AimInput.x;

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();
            _surfMovement.Tick(horizontalInput, groundInfo, _surfConfig, _boost, deltaTime);
            _boost = 0;
            _surfVisuals.UpdateSurfVisuals(groundInfo, _surfMovement.CurrentVelocity, horizontalInput, deltaTime);

            _protagCamera.UpdateProtagCamera(
                horizontalInput,
                deltaTime,
                _surfMovement.CurrentVelocity);

            _animator.SetBool("IsGrounded", groundInfo.IsGrounded);
            _animator.SetBool("FanOpen", Protaganist.IsFanOpen);

            bool isSurfing = groundInfo.IsGrounded;

            if (isSurfing)
            {
                _featherSystem.RefillFeathers();
            }

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