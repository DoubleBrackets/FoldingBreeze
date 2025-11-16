using Input;
using UnityEngine;

namespace UI
{
    public class FanOrientationCalibration : MonoBehaviour
    {
        public const string DefaultOrientation = "DefaultOrientation";

        private void Start()
        {
            LoadDefaultOrientation();
        }

        private void LoadDefaultOrientation()
        {
            if (PlayerPrefs.HasKey(DefaultOrientation))
            {
                string savedOrientationJson = PlayerPrefs.GetString(DefaultOrientation);
                var savedOrientation = JsonUtility.FromJson<Quaternion>(savedOrientationJson);
                SetDefaultOrientation(savedOrientation);
            }
        }

        public void SetOrientationToCurrent()
        {
            Quaternion currentOrientation = HandFanInputProvider.Instance.SetDefaultToCurrent();
            PlayerPrefs.SetString(DefaultOrientation, JsonUtility.ToJson(currentOrientation));
        }

        private void SetDefaultOrientation(Quaternion defaultRawOrientation)
        {
            HandFanInputProvider.Instance.ZeroedRawOrientation = defaultRawOrientation;
        }
    }
}