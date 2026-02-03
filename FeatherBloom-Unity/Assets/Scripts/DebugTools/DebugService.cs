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

                string lastPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs + "FoldingFan", string.Empty);
                if (!string.IsNullOrEmpty(lastPort))
                {
                    HandFanArduinoComm.Instance.SetSerialPort(lastPort);
                    HandFanArduinoComm.Instance.Connect();
                }

                lastPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs + "BoxFan", string.Empty);
                if (!string.IsNullOrEmpty(lastPort))
                {
                    BoxFanArduinoComm.Instance.SetSerialPort(lastPort);
                    BoxFanArduinoComm.Instance.Connect();
                }
            }
        }
    }
}