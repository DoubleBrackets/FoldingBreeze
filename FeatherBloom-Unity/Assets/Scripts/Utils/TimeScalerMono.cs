using Framework;
using Framework.GlobalServices;
using UnityEngine;

namespace Utils
{
    public class TimeScalerMono : MonoBehaviour
    {
        [SerializeField]
        private TimeScaleService.TimeScaleEntryConfig _timeScaleEntryConfig;

        public void Apply()
        {
            ServiceLocator.GetService<TimeScaleService>().NewTimeScaling(_timeScaleEntryConfig);
        }
    }
}