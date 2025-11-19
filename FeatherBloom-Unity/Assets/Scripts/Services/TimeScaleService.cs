using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class TimeScaleService : MonoBehaviour
    {
        [Serializable]
        public struct TimeScaleEntryConfig
        {
            public float Duration;
            public float ScaleFactor;
        }

        public struct TimeScaleEntry : IComparable<TimeScaleEntry>
        {
            public float EndTimeRealtime;
            public float ScaleFactor;

            public int CompareTo(TimeScaleEntry other)
            {
                return EndTimeRealtime.CompareTo(other.EndTimeRealtime);
            }
        }

        public static TimeScaleService Instance;

        private List<TimeScaleEntry> _entries = new();

        private float _defaultFixedDeltaTime;

        private void Awake()
        {
            Instance = this;
            _defaultFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Update()
        {
            float currentTime = Time.realtimeSinceStartup;

            var didChange = false;
            for (var i = 0; i < _entries.Count; i++)
            {
                TimeScaleEntry entryConfig = _entries[i];
                if (currentTime < entryConfig.EndTimeRealtime)
                {
                    break;
                }

                _entries.RemoveAt(i);
                i--;
                didChange = true;
            }

            if (didChange)
            {
                RecalculateTimeScale();
            }
        }

        public void NewTimeScaling(float factor, float duration)
        {
            AddTimeScaling(new TimeScaleEntry
            {
                ScaleFactor = factor,
                EndTimeRealtime = Time.realtimeSinceStartup + duration
            });
        }

        public void NewTimeScaling(TimeScaleEntryConfig entryConfig)
        {
            AddTimeScaling(new TimeScaleEntry
            {
                EndTimeRealtime = Time.realtimeSinceStartup + entryConfig.Duration,
                ScaleFactor = entryConfig.ScaleFactor
            });
        }

        private void AddTimeScaling(TimeScaleEntry entry)
        {
            _entries.Add(entry);
            _entries.Sort();

            RecalculateTimeScale();
        }

        private void RecalculateTimeScale()
        {
            float ratio = 1;
            foreach (TimeScaleEntry factor in _entries)
            {
                ratio *= factor.ScaleFactor;
            }

            Time.timeScale = ratio;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * ratio;
        }
    }
}