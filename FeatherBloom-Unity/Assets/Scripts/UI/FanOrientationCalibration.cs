using Framework;
using Input;
using UnityEngine;

namespace UI
{
    public class FanOrientationCalibration : MonoBehaviour
    {
        public void SetOrientationToCurrent()
        {
            ServiceLocator.GetService<GameplayInputService>().SetZeroedOrientationToCurrent();
        }
    }
}