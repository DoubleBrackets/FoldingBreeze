using System.Collections.Generic;
using UnityEngine;

namespace DevTools
{
    public class OnGUIHook : MonoBehaviour
    {
        private static Dictionary<string, string> _guiElements = new();

        private void Awake()
        {
            _guiElements.Clear();
        }

        public static void SetElement(string key, string value)
        {
            if (!_guiElements.TryAdd(key, value))
            {
                _guiElements[key] = value;
            }
        }

        public void OnGUI()
        {
            foreach (KeyValuePair<string, string> element in _guiElements)
            {
                GUILayout.Label($"{element.Key}: {element.Value}");
            }
        }
    }
}