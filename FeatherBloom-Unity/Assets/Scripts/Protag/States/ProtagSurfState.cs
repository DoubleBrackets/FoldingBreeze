using Protag.Surfing;
using UnityEngine;

namespace Protag.States
{
    public class ProtagSurfState : ProtagState
    {
        [SerializeField]
        private SurfMovement _surfMovement;

        [SerializeField]
        private SurfVisuals _surfVisuals;

        public override bool CanReenter { get; protected set; } = false;
        public override bool CanEnter { get; protected set; } = true;

        public void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            float horizontalInput = Protaganist.AimInput.x;
            SurfMovement.GroundedInfo groundInfo = _surfMovement.CheckGrounded();
            _surfMovement.Tick(horizontalInput, groundInfo, deltaTime);
            _surfVisuals.UpdateSurfVisuals(groundInfo, _surfMovement.CurrentVelocity, horizontalInput, deltaTime);
        }
    }
}