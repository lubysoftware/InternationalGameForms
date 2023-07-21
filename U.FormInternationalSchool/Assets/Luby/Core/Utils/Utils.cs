using System;
using UnityEngine;

namespace LubyLib.Core
{
    public static class Utils
    {
        /// <summary>
        ///     <para>Converts an int into a bool.</para>
        /// </summary>
        /// <param name="value"></param>
        public static bool IntToBool(int value)
        {
            return Convert.ToBoolean(value);
        }

        /// <summary>
        ///     <para>Converts a bool into a int.</para>
        /// </summary>
        /// <param name="value"></param>
        public static int BoolToInt(bool value)
        {
            return Convert.ToInt32(value);
        }
        
        /// <summary>
        ///     <para>Returns if value is between or equal to min and max.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        public static bool Between(float min, float max, float value)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        ///     <para>Returns a normalized number of the value with max as reference.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Normalize(float value, float max)
        {
            if (value == 0f)
            {
                return 0f;
            }

            return value / max;
        }

        /// <summary>
        ///     <para>Returns the direction Vector2 of the angle.</para>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 AngleToDirection(float angle) {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }
        
        /// <summary>
        ///     <para>Returns scale1Value in a different scale.</para>
        /// </summary>
        /// <param name="scale1Value"></param>
        /// <param name="scale1Min"></param>
        /// <param name="scale1Max"></param>
        /// <param name="scale2Min"></param>
        /// <param name="scale2Max"></param>
        public static float ConvertScales(float scale1Value, float scale1Min, float scale1Max, float scale2Min = 0, float scale2Max = 100 )
        {
            return ((scale1Value - scale1Min) * (scale2Max - scale2Min) / (scale1Max - scale1Min)) + scale2Min;
        }
    }
}
