using System.Collections.Generic;
using System.IO.Ports;
using Input.SerialComms;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using ValueSO;
using ValueSO.Core;

namespace UI.InputUI
{
    public class SerialPortDropdown : MonoBehaviour, IValueSOObserver
    {
        [Header("ValueSO (Read/Write)")]

        [SerializeField]
        private StringValueSO _lastPort;

        [Header("ValueSO (Read)")]

        [SerializeField]
        private StringValueSO _status;

        [Header("Event (Out)")]

        [SerializeField]
        private UnityEvent<string> onStatusChange;

        [Header("UI")]

        [SerializeField]
        private TMP_Dropdown dropdown;

        private IArduinoCom _arduinoCom;

        private void Start()
        {
            RefreshDropdown();
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            _status.AddListener(this, OnStatusChange, true);
        }

        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            _status.RemoveListener(this);
        }

        public void Initialize(IArduinoCom arduinoCom)
        {
            _arduinoCom = arduinoCom;
        }

        private void OnStatusChange(string status)
        {
            onStatusChange?.Invoke(status);
        }

        public void RefreshDropdown()
        {
            dropdown.ClearOptions();
            var options = new List<string>(SerialPort.GetPortNames());

            string lastPort = _lastPort.Value;

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
            _lastPort.SetValue(selectedPort);
        }

        public void Connect()
        {
            OnDropdownValueChanged(dropdown.value);
            _arduinoCom.Connect();
        }
    }
}