using UnityEngine;
using ValueSO;
using ValueSO.Core;

namespace Saving
{
    /// <summary>
    ///     Handles initial loading data from save model into ValueSOs, and writing changes back to save model
    /// </summary>
    [CreateAssetMenu(fileName = "SaveDataValueSOLoader", menuName = "SaveDataValueSOLoader")]
    public class SaveDataValueSOLoader : ScriptableObject, IValueSOObserver
    {
        [Header("ValueSO (Read/Write)")]

        [Header("Input Prefs")]

        [SerializeField]
        private StringValueSO _lastHandFanConnectedSerialPortNameValueSO;

        [SerializeField]
        private StringValueSO _lastBoxFanConnectedSerialPortNameValueSO;

        [SerializeField]
        private QuaternionValueSO _defaultZeroedOrientationValueSO;

        [Header("Score Saved")]

        [SerializeField]
        private IntValueSO _highScoreValueSO;

        /// <summary>
        ///     Setup ValueSOs to write changes back to save model
        /// </summary>
        /// <param name="saveService"></param>
        public void InitializeSaveBack(SaveService saveService)
        {
            _lastHandFanConnectedSerialPortNameValueSO.AddListener(this,
                value => saveService.LastHandFanConnectedSerialPortName = value);
            _lastBoxFanConnectedSerialPortNameValueSO.AddListener(this,
                value => saveService.LastBoxFanConnectedSerialPortName = value);
            _highScoreValueSO.AddListener(this, value => saveService.HighScore = value);
            _defaultZeroedOrientationValueSO.AddListener(this,
                value => saveService.DefaultZeroedOrientation = value);
        }

        public void Cleanup()
        {
            _lastHandFanConnectedSerialPortNameValueSO.RemoveListener(this);
            _lastBoxFanConnectedSerialPortNameValueSO.RemoveListener(this);
            _highScoreValueSO.RemoveListener(this);
            _defaultZeroedOrientationValueSO.RemoveListener(this);
        }

        public void Load(SaveModel saveModel)
        {
            _lastHandFanConnectedSerialPortNameValueSO.SetValue(saveModel.LastHandFanConnectedSerialPortName, this);
            _lastBoxFanConnectedSerialPortNameValueSO.SetValue(saveModel.LastBoxFanConnectedSerialPortName, this);
            _highScoreValueSO.SetValue(saveModel.HighScore, this);
            _defaultZeroedOrientationValueSO.SetValue(saveModel.DefaultZeroedOrientation, this);
        }
    }
}