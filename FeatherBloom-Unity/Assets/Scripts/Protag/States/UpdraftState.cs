using Events;
using Protag.Surfing;
using Services;
using UnityEngine;

namespace Protag.States
{
    public class UpdraftState : ProtagState
    {
        [SerializeField]
        private SurfMovement _surfMovement;

        [SerializeField]
        private GroundChecker _groundChecker;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private InteractableDetector _interactableDetector;

        [Header("Stateout")]

        [SerializeField]
        private ProtagState _airState;

        [SerializeField]
        private ProtagState _groundState;

        [Header("Config")]

        [SerializeField]
        private float _updraftVelocity;

        [SerializeField]
        private AnimationCurve _updraftVelocityCurve;

        [SerializeField]
        private float _duration;

        [SerializeField]
        private float _horizontalVelocityKeepRatio;

        [SerializeField]
        private TimeScaleService.TimeScaleEntryConfig _onEnterTimeScale;

        [Header("Event Out")]

        [SerializeField]
        private VoidEvent _onUpdraft;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        private float _stateTimer;

        private Vector3 _horizontalVelocity;
        private Vector3 _launchNormal;

        public override void OnEnter()
        {
            base.OnEnter();
            _groundChecker.ForceUnground(0.1f);
            _onUpdraft.Raise();
            _stateTimer = _duration;
            _animator.SetBool("Updraft", true);

            GroundChecker.GroundedInfo groundInfo = _groundChecker.CheckGrounded();
            _launchNormal = groundInfo.GroundNormal;

            _horizontalVelocity = Vector3.ProjectOnPlane(_surfMovement.CurrentVelocity, _launchNormal)
                                  * _horizontalVelocityKeepRatio;

            UpdateVelocity(_launchNormal);

            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);

            TimeScaleService.Instance.NewTimeScaling(_onEnterTimeScale);
        }

        public override void OnExit()
        {
            base.OnExit();
            _animator.SetBool("Updraft", false);

            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);
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
            float t = 1 - _stateTimer / _duration;
            float ratio = Mathf.Clamp01(_updraftVelocityCurve.Evaluate(t));
            _surfMovement.SetVelocity(normal * _updraftVelocity * ratio + _horizontalVelocity);
        }
    }
}