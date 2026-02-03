using SerialComms;
using UnityEngine;

namespace UI
{
    public class HandFanDropdownAdapter : MonoBehaviour
    {
        [SerializeField]
        private SerialPortDropdown _serialPortDropdown;

        private void Start()
        {
            _serialPortDropdown.Initialize(HandFanArduinoComm.Instance);
        }
    }
}