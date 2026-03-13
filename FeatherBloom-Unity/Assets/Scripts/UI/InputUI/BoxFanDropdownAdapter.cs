using Framework;
using Input.SerialComms;
using UnityEngine;

namespace UI.InputUI
{
    public class BoxFanDropdownAdapter : MonoBehaviour
    {
        [SerializeField]
        private SerialPortDropdown _serialPortDropdown;

        private void Start()
        {
            _serialPortDropdown.Initialize(ServiceLocator.GetService<BoxFanArduinoComm>());
        }
    }
}