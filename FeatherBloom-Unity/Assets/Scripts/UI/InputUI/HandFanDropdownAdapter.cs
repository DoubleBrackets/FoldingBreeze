using Framework;
using Input.SerialComms;
using UnityEngine;

namespace UI.InputUI
{
    public class HandFanDropdownAdapter : MonoBehaviour
    {
        [SerializeField]
        private SerialPortDropdown _serialPortDropdown;

        private void Start()
        {
            _serialPortDropdown.Initialize(ServiceLocator.GetService<HandFanArduinoComm>());
        }
    }
}