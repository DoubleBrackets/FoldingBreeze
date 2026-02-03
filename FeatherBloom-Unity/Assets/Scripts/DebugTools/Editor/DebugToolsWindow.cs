using SerialComms;
using UI;
using UnityEditor;
using UnityEngine;

namespace DebugTools.Editor
{
    /// <summary>
    ///     Editor window to toggle debug options
    /// </summary>
    public class DebugToolsWindow : EditorWindow
    {
        private const string QuickConnectPrefs = "QuickConnectDebugPrefs";
        private const string BlockLoadMapOnStart = "BlockLoadMapOnStart";
        private const string AutoRestartOnDeath = "AutoRestartOnDeath";

        private void OnGUI()
        {
            GUILayout.Label("Debug Tools", EditorStyles.boldLabel);

            bool quickConnect = DebugState.QuickArduinoConnect;
            bool newQuickConnect = GUILayout.Toggle(quickConnect, "Quick Arduino Connect");
            DebugState.QuickArduinoConnect = newQuickConnect;
            if (newQuickConnect != quickConnect)
            {
                PlayerPrefs.SetInt(QuickConnectPrefs, DebugState.QuickArduinoConnect ? 1 : 0);
            }

            bool blockLoadMap = DebugState.DoNotLoadMapOnStart;
            bool newBlockLoadMap = GUILayout.Toggle(blockLoadMap, "Block Load Map On Start");
            DebugState.DoNotLoadMapOnStart = newBlockLoadMap;
            if (newBlockLoadMap != blockLoadMap)
            {
                PlayerPrefs.SetInt(BlockLoadMapOnStart, newBlockLoadMap ? 1 : 0);
            }

            bool autoRestart = DebugState.AutoRestartOnDeath;
            bool newAutoReset = GUILayout.Toggle(autoRestart, "AutoReset on Death");
            DebugState.AutoRestartOnDeath = newAutoReset;
            if (autoRestart != newAutoReset)
            {
                PlayerPrefs.SetInt(AutoRestartOnDeath, newAutoReset ? 1 : 0);
            }

            EditorGUILayout.Space();
            GUILayout.Label("Prefs", EditorStyles.boldLabel);
            string calibrationOrientation = PlayerPrefs.GetString(FanOrientationCalibration.DefaultOrientation);
            string savedFoldingFanPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs + "FoldingFan");
            string savedBoxFanPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs + "BoxFan");

            GUILayout.Label("Default input port: " + savedFoldingFanPort);
            GUILayout.Label("Box fan input port: " + savedBoxFanPort);
            GUILayout.Label("Default orientation: " + calibrationOrientation);
        }

        private void OnFocus()
        {
            DebugState.QuickArduinoConnect = PlayerPrefs.GetInt(QuickConnectPrefs, 0) == 1;
            DebugState.DoNotLoadMapOnStart = PlayerPrefs.GetInt(BlockLoadMapOnStart, 0) == 1;
            DebugState.AutoRestartOnDeath = PlayerPrefs.GetInt(AutoRestartOnDeath, 0) == 1;
        }

        // Basic debug window
        [MenuItem("Feather/Debug Tools")]
        public static void ShowWindow()
        {
            GetWindow<DebugToolsWindow>("Feather Debug Tools");
        }
    }
}