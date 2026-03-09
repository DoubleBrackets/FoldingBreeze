using Protag.FeatherSystem;
using Protag.Movement;
using Protag.Presentation;
using Protag.Surfing;
using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    public class ProtagDashSurfState : ProtagState
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
        private FeatherManager _featherManager;

        [Header("Config")]

        [SerializeField]
        private SurfConfigSO _surfConfig;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isSurfDashing;

        [SerializeField]
        private BoolValueSO _isSurfing;

        public override bool CanReenter => false;
        public override bool CanEnter => true;

        private float _boost;

        public override void OnInitialize()
        {
            _impactSaver.OnTerrainImpact.AddListener(HandleTerrainImpact);

            SetStateValues(false);
        }

        public override void OnDeinitialize()
        {
            _impactSaver.OnTerrainImpact.RemoveListener(HandleTerrainImpact);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _interactableDetector.OnBoostPickup.AddListener(HandleBoostPickup);

            _featherManager.RefillFeathers();

            SetStateValues(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            _interactableDetector.OnBoostPickup.RemoveListener(HandleBoostPickup);

            SetStateValues(false);
        }

        private void SetStateValues(bool val)
        {
            _isSurfDashing.SetValue(val);
            _isSurfing.SetValue(val);
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
            float deltaTime = Time.fixedDeltaTime;

            float horizontalInput = Protaganist.AimInput.x;

            GroundChecker.GroundedInfo groundInfo = _groundChecker.LastGroundedInfo;
            _surfMovement.Tick(horizontalInput, groundInfo, _surfConfig, _boost, deltaTime);
            _boost = 0;
            _surfVisuals.UpdateSurfVisuals(groundInfo, _surfMovement.CurrentVelocity, horizontalInput, deltaTime);

            _protagCamera.UpdateProtagCamera(
                horizontalInput,
                deltaTime,
                _surfMovement.CurrentVelocity);

            bool isSurfing = groundInfo.IsGrounded;

            _isSurfing.SetValue(isSurfing);
        }
    }
}