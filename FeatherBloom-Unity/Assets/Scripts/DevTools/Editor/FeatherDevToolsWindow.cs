using DevTools;
using Framework.LevelLoading;
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

        private void OnBecameVisible()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnBecameInvisible()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            MeasuringTool();
        }

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

            GameLevelSO overrideLevel = DevToolState.OverrideStartupLevel;
            DevToolState.OverrideStartupLevel = (GameLevelSO)EditorGUILayout.ObjectField("Override Startup Level",
                overrideLevel, typeof(GameLevelSO), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Persistent Data Path"))
            {
                EditorUtility.RevealInFinder(Application.persistentDataPath);
            }
        }

        private void MeasuringTool()
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length == 2)
            {
                GameObject obj1 = selectedObjects[0];
                GameObject obj2 = selectedObjects[1];
                float distance = Vector3.Distance(obj1.transform.position, obj2.transform.position);
                Handles.Label((obj1.transform.position + obj2.transform.position) / 2f, $"Distance: {distance:F2}");
                Handles.DrawLine(obj1.transform.position, obj2.transform.position);
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