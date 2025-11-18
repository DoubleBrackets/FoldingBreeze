using UnityEngine;

namespace Utils
{
    public static class ExtensionMethods
    {
        public static bool IsInMask(this int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) > 0;
        }

        /// <summary>
        ///     Remap [-1,1] to [0,1]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float RemapOnesTo01(this float value)
        {
            return value * 0.5f + 0.5f;
        }
    }
}