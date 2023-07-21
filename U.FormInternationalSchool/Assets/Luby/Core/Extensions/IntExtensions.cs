using UnityEngine;

namespace LubyLib.Core.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        ///   <para>Returns the absolute value of value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Abs.html">External documentation for `Mathf.Abs`</a></footer>
        public static int Abs(this int value)
        {
            return Mathf.Abs(value);
        }
        
        /// <summary>
        ///   <para>Clamps the given value between a range defined by the given minimum integer and maximum integer values. Returns the given value if it is within min and max.</para>
        /// </summary>
        /// <param name="value">The integer point value to restrict inside the min-to-max range</param>
        /// <param name="min">The minimum integer point value to compare against.</param>
        /// <param name="max">The maximum  integer point value to compare against.</param>
        /// <returns>
        ///   <para>The int result between min and max values.</para>
        /// </returns>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Clamp.html">External documentation for `Mathf.Clamp`</a></footer>
        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Clamp( value, min, max);
        }
        /// <summary>
        ///   <para>Clamps value between 0 and 1 and returns value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Clamp01.html">External documentation for `Mathf.Clamp01`</a></footer>
        public static int Clamp01(this int value)
        {
            return Mathf.Clamp(value,0,1);
        }

        /// <summary>
        ///   <para>Returns f rounded to the nearest step as a integer.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static int RoundStep(this int value, int step)
        {
            float steps = value / (float)step;
            return Mathf.RoundToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns f rounded to the nearest step.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static float RoundStep(this int value, float step)
        {
            float steps = value / step;
            return Mathf.RoundToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns the largest integer on the step table rounded up.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static int CeilStep(this int value, int step)
        {
            float steps = value / (float)step;
            return Mathf.CeilToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns the largest number on the step table rounded up.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static float CeilStep(this int value, float step)
        {
            float steps = value / step;
            return Mathf.CeilToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns the largest integer on the step table rounded down.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static int FloorStep(this int value, int step)
        {
            float steps = value / (float)step;
            return Mathf.FloorToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns the largest number on the step table rounded down.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public static float FloorStep(this int value, float step)
        {
            float steps = value / step;
            return Mathf.FloorToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns if value is pair.</para>
        /// </summary>
        /// <param name="value"></param>
        public static bool IsPair(this int value)
        {
            return value % 2 == 0;
        }
        /// <summary>
        ///   <para>Returns if value is multiple of num.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        public static bool IsMultiple(this int value, float num)
        {
            return value % num == 0;
        }
        
        /// <summary>
        ///   <para>Returns value raised to power p.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="p"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Pow.html">External documentation for `Mathf.Pow`</a></footer>
        public static float Pow(this int value, float p)
        {
            return Mathf.Pow(value, p);
        }
        /// <summary>
        ///   <para>Returns square root of value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Sqrt.html">External documentation for `Mathf.Sqrt`</a></footer>
        public static float Sqrt(this int value)
        {
            return Mathf.Sqrt(value);
        }
        
        /// <summary>
        ///     <para>Returns scale1Value in a different scale.</para>
        /// </summary>
        /// <param name="scale1Value"></param>
        /// <param name="scale1Min"></param>
        /// <param name="scale1Max"></param>
        /// <param name="scale2Min"></param>
        /// <param name="scale2Max"></param>
        public static float ConvertScales(this int scale1Value, float scale1Min, float scale1Max, float scale2Min = 0, float scale2Max = 100)
        {
            return Utils.ConvertScales(scale1Value, scale1Min, scale1Max, scale2Min, scale2Max);
        }

        public static float SmoothDamp(this int number, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime);
        }
        public static float SmoothDamp(this int number, float target, ref float velocity, float smoothTime, float maxSpeed)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime, maxSpeed);
        }
        public static float SmoothDamp(this int number, float target, ref float velocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime, maxSpeed, deltaTime);
        }
    }
}