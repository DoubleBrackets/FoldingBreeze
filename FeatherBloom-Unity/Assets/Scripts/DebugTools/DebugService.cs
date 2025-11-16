using Input;
using SerialComms;
using UnityEngine;

namespace DebugTools
{
    /// <summary>
    ///     Service that handles debug tool functionality
    /// </summary>
    public class DebugService : MonoBehaviour
    {
        private void Start()
        {
            if (DebugState.QuickArduinoConnect)
            {
                GameplayInputService.Instance.SwitchInputType(GameplayInputService.GameplayInputType.CustomHardware);

                string lastPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs, string.Empty);
                if (!string.IsNullOrEmpty(lastPort))
                {
                    HandFanArduinoComm.Instance.SetSerialPort(lastPort);
                    HandFanArduinoComm.Instance.Connect();
                }
            }
        }
    }
}