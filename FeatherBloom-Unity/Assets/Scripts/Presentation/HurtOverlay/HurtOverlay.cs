using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Presentation.HurtOverlay
{
    public class HurtOverlay : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private CanvasGroup _hurtOverlayCanvas;

        [Header("ValueSO (Read)")]

        [SerializeField]
        private IntValueSO _healthValueSO;

        private float _desiredAlpha;

        private void Awake()
        {
            _healthValueSO.AddListener(this, HandleHealthChange, true);
        }

        private void OnDestroy()
        {
            _healthValueSO.RemoveListener(this);
        }

        private void HandleHealthChange(int health)
        {
            _desiredAlpha = health > 1 ? 0f : 1f;
        }

        private void FixedUpdate()
        {
            if (_hurtOverlayCanvas.alpha < _desiredAlpha)
            {
                _hurtOverlayCanvas.alpha = _desiredAlpha;
            }
            else
            {
                _hurtOverlayCanvas.alpha =
                    Mathf.Lerp(_hurtOverlayCanvas.alpha, _desiredAlpha, Time.fixedDeltaTime * 4f);
            }
        }
    }
}