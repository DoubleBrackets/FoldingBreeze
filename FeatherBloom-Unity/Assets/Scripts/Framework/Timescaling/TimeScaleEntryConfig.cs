using System;

namespace Framework.Timescaling
{
    [Serializable]
    public struct TimeScaleEntryConfig
    {
        public float Duration;
        public float ScaleFactor;
        public string Identifier;
    }
}