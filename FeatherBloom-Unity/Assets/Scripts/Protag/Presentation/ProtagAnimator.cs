using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Protag.Presentation
{
    /// <summary>
    ///     Handles protaganist animation control
    /// </summary>
    public class ProtagAnimator : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private Animator _animator;

        [Header("ValueSO (Read)")]

        [SerializeField]
        private BoolValueSO _isGroundedValueSO;

        [SerializeField]
        private BoolValueSO _fanOpenValueSO;

        [SerializeField]
        private BoolValueSO _isUpdraftingValueSO;

        [SerializeField]
        private BoolValueSO _isDeadValueSO;

        private void Start()
        {
            _isGroundedValueSO.AddListener(this, HandleGroundedChange, true);
            _fanOpenValueSO.AddListener(this, HandleFanOpenChange, true);
            _isUpdraftingValueSO.AddListener(this, HandleUpdraftingChange, true);
            _isDeadValueSO.AddListener(this, HandleDeadChange, true);
        }

        private void HandleDeadChange(bool isDead)
        {
            _animator.speed = isDead ? 0f : 1f;
        }

        private void OnDestroy()
        {
            _isGroundedValueSO.RemoveListener(this);
            _fanOpenValueSO.RemoveListener(this);
            _isUpdraftingValueSO.RemoveListener(this);
            _isDeadValueSO.RemoveListener(this);
        }

        private void HandleGroundedChange(bool isGrounded)
        {
            _animator.SetBool("IsGrounded", isGrounded);
        }

        private void HandleFanOpenChange(bool isFanOpen)
        {
            _animator.SetBool("FanOpen", isFanOpen);
        }

        private void HandleUpdraftingChange(bool isUpdrafting)
        {
            _animator.SetBool("Updraft", isUpdrafting);
        }
    }
}