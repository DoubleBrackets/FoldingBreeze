using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SerialComms
{
    public class SerialPortDropdown : MonoBehaviour
    {
        public const string LastPortPrefs = "LastSerialPortSelected";

        [SerializeField]
        private TMP_Dropdown dropdown;

        [SerializeField]
        private UnityEvent<string> onStatusChange;

        [SerializeField]
        private string _playerPrefsPostfix;

        private string PlayerPrefsKey => LastPortPrefs + _playerPrefsPostfix;

        private IArduinoCom _arduinoCom;

        private void Start()
        {
            RefreshDropdown();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            _arduinoCom.OnStatusChangeEvent.RemoveListener(OnStatusChange);
        }

        public void Initialize(IArduinoCom arduinoCom)
        {
            _arduinoCom = arduinoCom;
            _arduinoCom.OnStatusChangeEvent.AddListener(OnStatusChange);
        }

        private void OnStatusChange(string status)
        {
            onStatusChange?.Invoke(status);
        }

        public void RefreshDropdown()
        {
            dropdown.ClearOptions();
            var options = new List<string>(SerialPort.GetPortNames());

            string lastPort = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
            if (!string.IsNullOrEmpty(lastPort) && options.Contains(lastPort))
            {
                options.Remove(lastPort);
                options.Insert(0, lastPort);
            }

            dropdown.AddOptions(options);
        }

        private void OnDropdownValueChanged(int index)
        {
            string selectedPort = dropdown.options[index].text;
            _arduinoCom.SetSerialPort(selectedPort);
            PlayerPrefs.SetString(PlayerPrefsKey, selectedPort);
            PlayerPrefs.Save();
        }

        public void Connect()
        {
            OnDropdownValueChanged(dropdown.value);
            _arduinoCom.Connect();
        }
    }
}