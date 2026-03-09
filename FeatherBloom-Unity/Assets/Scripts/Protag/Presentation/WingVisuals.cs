using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Protag.Presentation
{
    public class WingVisuals : MonoBehaviour, IValueSOObserver
    {
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private string _featherResourceShaderParam;

        [SerializeField]
        private float _lerpFactor;

        [Header("ValueSO (Read)")]

        [SerializeField]
        private FloatValueSO _featherResourceValue;

        [SerializeField]
        private BoolValueSO _isFanOpen;

        private MaterialPropertyBlock _materialPropertyBlock;

        private float _target;
        private float _shownValue;

        private void Awake()
        {
            _featherResourceValue.AddListener(this, HandleFeatherResourceChange, true);
            _isFanOpen.AddListener(this, HandleFanOpenChange, true);
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        private void HandleFanOpenChange(bool isOpen)
        {
            _renderer.enabled = isOpen;
        }

        private void OnDestroy()
        {
            _featherResourceValue.RemoveListener(this);
            _isFanOpen.RemoveListener(this);
        }

        private void Update()
        {
            float t = 1 - Mathf.Pow(0.01f, Time.deltaTime * _lerpFactor);
            _shownValue = Mathf.Lerp(_shownValue, _target, t);
            _materialPropertyBlock.SetFloat(_featherResourceShaderParam, _shownValue);
            _renderer.SetPropertyBlock(_materialPropertyBlock, 0);
            _renderer.SetPropertyBlock(_materialPropertyBlock, 1);
        }

        private void HandleFeatherResourceChange(float value)
        {
            _target = value;
        }
    }
}