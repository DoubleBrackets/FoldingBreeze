using DevTools;
using UnityEngine;
using ValueSO.Core;

namespace Protag.FeatherSystem
{
    /// <summary>
    ///     Flight resource system
    /// </summary>
    public class FeatherSystem : MonoBehaviour
    {
        [Header("ValueSO (Write)")]

        [SerializeField]
        private FloatValueSO _featherValue;

        /// <summary>
        ///     Amount of feather remaining. Value between 0 and 1
        /// </summary>
        private float _currentFeatherValue;

        public float FeatherValue => _currentFeatherValue;

        /// <summary>
        ///     Try to consume feathers, returns true if there are enough feathers
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool TryConsumeFeathers(float amount)
        {
            if (_currentFeatherValue - amount < 0f)
            {
                return false;
            }

            SetFeatherValueInternal(_currentFeatherValue - amount);
            return true;
        }

        public void RefillFeathers()
        {
            SetFeatherValueInternal(1);
        }

        private void SetFeatherValueInternal(float value)
        {
            _currentFeatherValue = Mathf.Clamp(value, 0f, 1f);
            _featherValue.SetValue(_currentFeatherValue);
            OnGUIHook.SetElement("Feather Value", _currentFeatherValue.ToString());
        }
    }
}