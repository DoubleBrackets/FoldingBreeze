using UnityEngine;
using ValueSO.Core;

namespace Protag.States
{
    public class HealingState : ProtagState
    {
        [Header("Config")]

        [SerializeField]
        private float _healingDuration;

        [Header("ValueSO (Write)")]

        [SerializeField]
        private BoolValueSO _isHealingValueSO;

        private float _stateTimer;

        public bool IsFinished => _stateTimer <= 0f;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _isHealingValueSO.SetValue(false);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _isHealingValueSO.SetValue(true);
            _stateTimer = _healingDuration;

            Protaganist.Heal();
        }

        public override void OnExit()
        {
            base.OnExit();
            _isHealingValueSO.SetValue(false);
        }

        private void FixedUpdate()
        {
            _stateTimer -= Time.fixedDeltaTime;
        }

        public override bool CanReenter => false;
        public override bool CanEnter => Protaganist.Health < Protaganist.MaxHealth;
    }
}