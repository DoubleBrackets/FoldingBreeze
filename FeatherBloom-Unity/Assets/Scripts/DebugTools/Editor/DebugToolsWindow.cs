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
        private const string quickConnectPrefs = "QuickConnectDebugPrefs";

        private void OnGUI()
        {
            GUILayout.Label("Debug Tools", EditorStyles.boldLabel);

            bool quickConnect = DebugState.QuickArduinoConnect;
            bool newQuickConnect = GUILayout.Toggle(quickConnect, "Quick Arduino Connect");
            DebugState.QuickArduinoConnect = newQuickConnect;

            if (newQuickConnect != quickConnect)
            {
                PlayerPrefs.SetInt(quickConnectPrefs, DebugState.QuickArduinoConnect ? 1 : 0);
            }

            EditorGUILayout.Space();
            GUILayout.Label("Prefs", EditorStyles.boldLabel);
            string calibrationOrientation = PlayerPrefs.GetString(FanOrientationCalibration.DefaultOrientation);
            string savedPort = PlayerPrefs.GetString(SerialPortDropdown.LastPortPrefs);

            GUILayout.Label("Default port: " + savedPort);
            GUILayout.Label("Default orientation: " + calibrationOrientation);
        }

        private void OnFocus()
        {
            DebugState.QuickArduinoConnect = PlayerPrefs.GetInt(quickConnectPrefs, 0) == 1;
        }

        // Basic debug window
        [MenuItem("Feather/Debug Tools")]
        public static void ShowWindow()
        {
            GetWindow<DebugToolsWindow>("Feather Debug Tools");
        }
    }
}