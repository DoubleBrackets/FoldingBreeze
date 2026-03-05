using Framework;
using Framework.Timescaling;
using UnityEngine;

namespace Utils
{
    public class TimeScalerMono : MonoBehaviour
    {
        [SerializeField]
        private TimeScaleEntryConfig _timeScaleEntryConfig;

        public void Apply()
        {
            ServiceLocator.GetService<TimeScaleService>().NewTimeScaling(_timeScaleEntryConfig);
        }
    }
}