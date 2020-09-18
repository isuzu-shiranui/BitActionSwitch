#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace BitActionSwitch.Editor.Utility
{
    public static class BitUtil
    {
        public static int ToInt(this IReadOnlyList<bool> array)
        {
            var defaultInt = 0;
            for (var i = 0; i < array.Count; i++)
            {
                if (array[i]) defaultInt += 1 << i;
            }

            return defaultInt;
        }
        
        public static bool[] ToBinaryArray(this int number, int digit)
        {
            var ret = new bool[digit];
            for (var i = 0; i < digit; i++)
            {
                ret[i] = (1 << i & number) != 0;
            }
            return ret;
        }
    }
}
#endif