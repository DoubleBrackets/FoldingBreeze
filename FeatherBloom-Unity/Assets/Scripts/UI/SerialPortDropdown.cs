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

        private void Start()
        {
            RefreshDropdown();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            HandFanArduinoComm.Instance.OnStatusChange.AddListener(OnStatusChange);
        }

        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            HandFanArduinoComm.Instance.OnStatusChange.RemoveListener(OnStatusChange);
        }

        private void OnStatusChange(string status)
        {
            onStatusChange?.Invoke(status);
        }

        public void RefreshDropdown()
        {
            dropdown.ClearOptions();
            var options = new List<string>(SerialPort.GetPortNames());

            string lastPort = PlayerPrefs.GetString(LastPortPrefs, string.Empty);
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
            HandFanArduinoComm.Instance.SetSerialPort(selectedPort);
            PlayerPrefs.SetString(LastPortPrefs, selectedPort);
            PlayerPrefs.Save();
        }

        public void Connect()
        {
            OnDropdownValueChanged(dropdown.value);
            HandFanArduinoComm.Instance.Connect();
        }
    }
}