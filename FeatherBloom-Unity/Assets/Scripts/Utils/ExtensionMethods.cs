using UnityEngine;

namespace Utils
{
    public static class ExtensionMethods
    {
        public static bool IsInMask(this int layer, LayerMask mask)
        {
            return (mask.value & (1 << layer)) > 0;
        }
    }
}