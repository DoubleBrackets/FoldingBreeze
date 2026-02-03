using Services;
using UnityEngine;

namespace Utils
{
    public class TimeScalerMono : MonoBehaviour
    {
        [SerializeField]
        private TimeScaleService.TimeScaleEntryConfig _timeScaleEntryConfig;

        public void Apply()
        {
            TimeScaleService.Instance.NewTimeScaling(_timeScaleEntryConfig);
        }
    }
}