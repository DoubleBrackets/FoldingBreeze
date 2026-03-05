using DevTools;
using UnityEditor;
using UnityEngine;

namespace DebugTools.Editor
{
    /// <summary>
    ///     Game dev tools window
    /// </summary>
    public class FeatherDevToolsWindow : EditorWindow
    {
        private const string QuickConnectPrefs = "QuickConnectDebugPrefs";
        private const string BlockLoadMapOnStart = "BlockLoadMapOnStart";
        private const string AutoRestartOnDeath = "AutoRestartOnDeath";
        private const string GoIntoGameplayPrefs = "GoIntoGameplayPrefs";

        private void OnGUI()
        {
            GUILayout.Label("Debug Tools", EditorStyles.boldLabel);

            bool quickConnect = DevToolState.QuickArduinoConnect;
            bool newQuickConnect = GUILayout.Toggle(quickConnect, "Quick Arduino Connect");
            DevToolState.QuickArduinoConnect = newQuickConnect;
            if (newQuickConnect != quickConnect)
            {
                EditorPrefs.SetInt(QuickConnectPrefs, DevToolState.QuickArduinoConnect ? 1 : 0);
            }

            bool blockLoadMap = DevToolState.DoNotLoadMapOnStart;
            bool newBlockLoadMap = GUILayout.Toggle(blockLoadMap, "Block Load Map On Start");
            DevToolState.DoNotLoadMapOnStart = newBlockLoadMap;
            if (newBlockLoadMap != blockLoadMap)
            {
                EditorPrefs.SetInt(BlockLoadMapOnStart, newBlockLoadMap ? 1 : 0);
            }

            bool autoRestart = DevToolState.AutoRestartOnDeath;
            bool newAutoReset = GUILayout.Toggle(autoRestart, "AutoReset on Death");
            DevToolState.AutoRestartOnDeath = newAutoReset;
            if (autoRestart != newAutoReset)
            {
                EditorPrefs.SetInt(AutoRestartOnDeath, newAutoReset ? 1 : 0);
            }

            bool goIntoGameplay = DevToolState.GoIntoGameplayImmediately;
            bool newGoIntoGameplay = GUILayout.Toggle(goIntoGameplay, "Go Into Gameplay Immediately");
            DevToolState.GoIntoGameplayImmediately = newGoIntoGameplay;
            if (goIntoGameplay != newGoIntoGameplay)
            {
                EditorPrefs.SetInt(GoIntoGameplayPrefs, newGoIntoGameplay ? 1 : 0);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Persistent Data Path"))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }

        private void OnFocus()
        {
            DevToolState.QuickArduinoConnect = EditorPrefs.GetInt(QuickConnectPrefs, 0) == 1;
            DevToolState.DoNotLoadMapOnStart = EditorPrefs.GetInt(BlockLoadMapOnStart, 0) == 1;
            DevToolState.AutoRestartOnDeath = EditorPrefs.GetInt(AutoRestartOnDeath, 0) == 1;
        }

        // Basic debug window
        [MenuItem("Feather/Debug Tools")]
        public static void ShowWindow()
        {
            GetWindow<FeatherDevToolsWindow>("Feather Debug Tools");
        }
    }
}