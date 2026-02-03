using SerialComms;
using UnityEngine;

namespace UI
{
    public class BoxFanDropdownAdapter : MonoBehaviour
    {
        [SerializeField]
        private SerialPortDropdown _serialPortDropdown;

        private void Start()
        {
            _serialPortDropdown.Initialize(BoxFanArduinoComm.Instance);
        }
    }
}