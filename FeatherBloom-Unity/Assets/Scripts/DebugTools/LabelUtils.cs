using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DebugTools
{
    public class LabelUtils : MonoBehaviour
    {
        private static GUIStyle labelStyle;

        public static void Label(Vector3 position, string text)
        {
#if UNITY_EDITOR
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle();
                labelStyle.normal.textColor = Color.black;
                labelStyle.fontSize = 14;
            }

            Handles.Label(position, text, labelStyle);
#endif
        }
    }
}