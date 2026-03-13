using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    public class ProtagAscendState : ProtagState
    {
        [SerializeField]
        private Rigidbody _rb;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isAscendValueSO;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _isAscendValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _isAscendValueSO.SetValue(true);
            _rb.isKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override bool CanReenter => false;
        public override bool CanEnter => true;
    }
}