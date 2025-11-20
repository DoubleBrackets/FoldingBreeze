using System;
using System.Collections.Generic;
using UnityEngine;

namespace Input.FanInput
{
    /// <summary>
    ///     Basic heuristics to recognize gesture from fan input
    /// </summary>
    public class FanGestureRecognizer
    {
        public enum GestureTypes
        {
            Slice,
            Updraft,
            Gust,
            FanSelf
        }

        private float _debounceTime;

        /// <summary>
        ///     TODO: Can probably be minor optimized into linked list
        /// </summary>
        private List<GesturePoint> _gesturePointBuffer = new();

        private GestureRecognizeConfig _gestureRecognizeConfig;

        public FanGestureRecognizer(GestureRecognizeConfig config)
        {
            _gestureRecognizeConfig = config;
        }

        public event Action<GestureTypes> OnGestureTriggered;

        public void AddGesturePoint(Quaternion orientation, float realTimeStamp)
        {
            _gesturePointBuffer.Add(new GesturePoint
            {
                Orientation = orientation,
                RealTimeStamp = realTimeStamp
            });
        }

        public void ProcessGestures()
        {
            float currentTime = Time.realtimeSinceStartup;
            float bufferTime = _gestureRecognizeConfig.BufferTime;
            // Remove samples that are out of buffer window
            for (var i = 0; i < _gesturePointBuffer.Count; i++)
            {
                GesturePoint gesture = _gesturePointBuffer[i];
                if (gesture.RealTimeStamp >= currentTime - bufferTime)
                {
                    break;
                }

                _gesturePointBuffer.RemoveAt(0);
                i--;
            }

            if (_gesturePointBuffer.Count < 2)
            {
                Debug.LogWarning("Gesture unable to check; Buffer less than 2 data points");
                return;
            }

            // Calculate average angular velocity
            GesturePoint pointB = _gesturePointBuffer[^1];
            GesturePoint pointA = _gesturePointBuffer[0];
            float timeDelta = pointB.RealTimeStamp - pointA.RealTimeStamp;
            Quaternion angleDifference = Quaternion.Inverse(pointB.Orientation) * pointA.Orientation;

            angleDifference.ToAngleAxis(out float totalAngularDistance, out Vector3 axis);

            float angularVelocity = totalAngularDistance / timeDelta;
            if (angularVelocity > _gestureRecognizeConfig.ThresholdAngularVelocity)
            {
                if (currentTime < _debounceTime)
                {
                    return;
                }

                Quaternion averageOrientation = Quaternion.Lerp(pointA.Orientation, pointB.Orientation, 0.5f);
                bool recognized = RecognizeGesture(pointA.Orientation, pointB.Orientation, averageOrientation);

                if (recognized)
                {
                    _debounceTime = currentTime + _gestureRecognizeConfig.DebounceInterval;
                }
            }
        }

        private bool RecognizeGesture(Quaternion a, Quaternion b, Quaternion averageOrientation)
        {
            // Axis axis is the one the fan unfolds around
            // If fan's "axis" axis is perpendicular to gesture axis, then it's a "fanning" gesture
            // If fan's "axis" axis is parallel to gesture axis, then it's a "slicing" gesture
            // Note that this is when the fan is open, so the fan's left axis is +z (i.e forward) in game
            Vector3 gestureAxis = Vector3.Cross(a * Vector3.left, b * Vector3.left).normalized;
            Vector3 axleAxis = averageOrientation * Vector3.up;

            float dot = Vector3.Dot(axleAxis, gestureAxis);
            float angle = Mathf.Acos(Mathf.Abs(dot)) * Mathf.Rad2Deg;

            if (angle < 45)
            {
                OnGestureTriggered?.Invoke(GestureTypes.Slice);
                return true;
            }

            // Fanning self is any fanning motion facing backwards
            Vector3 leftAxis = averageOrientation * Vector3.left;
            if (leftAxis.z < 0)
            {
                OnGestureTriggered?.Invoke(GestureTypes.FanSelf);
                return true;
            }

            // Gust is fanning left/right forward
            float dotWithUp = Vector3.Dot(Vector3.up, gestureAxis);
            float upAngle = Mathf.Acos(Mathf.Abs(dotWithUp)) * Mathf.Rad2Deg;
            if (upAngle < 55)
            {
                OnGestureTriggered?.Invoke(GestureTypes.Gust);
                return true;
            }

            // Updraft is fanning up->down facing forward
            // cross prod sign indicates direction
            if (gestureAxis.x > 0)
            {
                OnGestureTriggered?.Invoke(GestureTypes.Updraft);
                return true;
            }

            Debug.Log("Unrecognized gesture");
            return false;
        }

        private struct GesturePoint
        {
            /// <summary>
            ///     Transformed orientation
            /// </summary>
            public Quaternion Orientation;

            public float RealTimeStamp;
        }

        [Serializable]
        public struct GestureRecognizeConfig
        {
            public float ThresholdAngularVelocity;
            public float BufferTime;
            public float DebounceInterval;
        }
    }
}