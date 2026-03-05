using System;
using System.Collections.Generic;
using UnityEngine;
using ValueSO.Core;

namespace Framework.GlobalServices
{
    /// <summary>
    ///     Manages time scaling for gameplay purposes
    /// </summary>
    public class TimeScaleService
    {
        [Serializable]
        public struct TimeScaleEntryConfig
        {
            public float Duration;
            public float ScaleFactor;
            public string Identifier;
        }

        public struct TimeScaleEntry : IComparable<TimeScaleEntry>
        {
            public float EndTimeRealtime;
            public float ScaleFactor;
            public string Identifier;

            public int CompareTo(TimeScaleEntry other)
            {
                return EndTimeRealtime.CompareTo(other.EndTimeRealtime);
            }
        }

        private FloatValueSO _lerpFactor;

        private List<TimeScaleEntry> _entries = new();

        private float _defaultFixedDeltaTime = Time.fixedDeltaTime;

        private float _desiredTimeScale = 1;
        private float _currentTimeScale = 1;

        public TimeScaleService(FloatValueSO lerpFactor)
        {
            _lerpFactor = lerpFactor;
        }

        public void DoUpdate()
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

            float t = 1 - Mathf.Pow(0.01f, Time.unscaledDeltaTime * _lerpFactor.Value);
            _currentTimeScale = Mathf.Lerp(_currentTimeScale, _desiredTimeScale, t);
            Time.timeScale = _currentTimeScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * _currentTimeScale;
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
                ScaleFactor = entryConfig.ScaleFactor,
                Identifier = entryConfig.Identifier ?? string.Empty
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

            _desiredTimeScale = ratio;
        }

        public void RemoveTimeScale(string identifier)
        {
            int foundIndex = _entries.FindIndex(a => a.Identifier == identifier);

            if (foundIndex != -1)
            {
                _entries.RemoveAt(foundIndex);
            }

            RecalculateTimeScale();
        }
    }
}