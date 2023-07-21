using UnityEngine;

namespace LubyLib.Core.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>
        ///   <para>Returns the absolute value of f.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Abs.html">External documentation for `Mathf.Abs`</a></footer>
        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }
        
        /// <summary>
        ///   <para>Clamps the given value between the given minimum float and maximum float values.  Returns the given value if it is within the min and max range.</para>
        /// </summary>
        /// <param name="value">The floating point value to restrict inside the range defined by the min and max values.</param>
        /// <param name="min">The minimum floating point value to compare against.</param>
        /// <param name="max">The maximum floating point value to compare against.</param>
        /// <returns>
        ///   <para>The float result between the min and max values.</para>
        /// </returns>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Clamp.html">External documentation for `Mathf.Clamp`</a></footer>
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp( value, min, max);
        }
        /// <summary>
        ///   <para>Clamps value between 0 and 1 and returns value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Clamp01.html">External documentation for `Mathf.Clamp01`</a></footer>
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }
        
        /// <summary>
        ///   <para>Returns f rounded to the nearest integer.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Round.html">External documentation for `Mathf.Round`</a></footer>
        public static float Round(this float f)
        {
            return Mathf.Round(f);
        }
        /// <summary>
        ///   <para>Returns the largest integer smaller than or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Floor.html">External documentation for `Mathf.Floor`</a></footer>
        public static float Floor(this float f)
        {
            return Mathf.Floor(f);
        }
        /// <summary>
        ///   <para>Returns the smallest integer greater to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Ceil.html">External documentation for `Mathf.Ceil`</a></footer>
        public static float Ceil(this float f)
        {
            return Mathf.Ceil(f);
        }
        
        /// <summary>
        ///   <para>Returns f rounded to the nearest integer.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.RoundToInt.html">External documentation for `Mathf.RoundToInt`</a></footer>
        public static int RoundToInt(this float f)
        {
            return Mathf.RoundToInt(f);
        }
        /// <summary>
        ///   <para>Returns the largest integer smaller to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.FloorToInt.html">External documentation for `Mathf.FloorToInt`</a></footer>
        public static int FloorToInt(this float f)
        {
            return Mathf.FloorToInt(f);
        }
        /// <summary>
        ///   <para>Returns the smallest integer greater to or equal to f.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.CeilToInt.html">External documentation for `Mathf.CeilToInt`</a></footer>
        public static int CeilToInt(this float f)
        {
            return Mathf.CeilToInt(f);
        }
        
        /// <summary>
        ///   <para>Returns f rounded to the nearest step as a integer.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static int RoundStep(this float f, int step)
        {
            float steps = f / step;
            return Mathf.RoundToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns f rounded to the nearest step.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static float RoundStep(this float f, float step)
        {
            float steps = f / step;
            return Mathf.RoundToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns the largest integer on the step table rounded up.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static int CeilStep(this float f, int step)
        {
            float steps = f / (float)step;
            return Mathf.CeilToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns the largest number on the step table rounded up.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static float CeilStep(this float f, float step)
        {
            float steps = f / step;
            return Mathf.CeilToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns the largest integer on the step table rounded down.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static int FloorStep(this float f, int step)
        {
            float steps = f / (float)step;
            return Mathf.FloorToInt(steps) * step;
        }
        /// <summary>
        ///   <para>Returns the largest number on the step table rounded down.</para>
        /// </summary>
        /// <param name="f"></param>
        /// <param name="step"></param>
        public static float FloorStep(this float f, float step)
        {
            float steps = f / step;
            return Mathf.FloorToInt(steps) * step;
        }
        
        /// <summary>
        ///   <para>Returns if value is pair.</para>
        /// </summary>
        /// <param name="value"></param>
        public static bool IsPair(this float value)
        {
            return value % 2 == 0;
        }
        /// <summary>
        ///   <para>Returns if value is multiple of num.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="num"></param>
        public static bool IsMultiple(this float value, float num)
        {
            return value % num == 0;
        }

        /// <summary>
        ///   <para>Returns value raised to power p.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="p"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Pow.html">External documentation for `Mathf.Pow`</a></footer>
        public static float Pow(this float value, float p)
        {
            return Mathf.Pow(value, p);
        }
        /// <summary>
        ///   <para>Returns square root of value.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <footer><a href="file:///C:/Program%20Files/Unity/Hub/Editor/2020.3.18f1/Editor/Data/Documentation/en/ScriptReference/Mathf.Sqrt.html">External documentation for `Mathf.Sqrt`</a></footer>
        public static float Sqrt(this float value)
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
        public static float ConvertScales(this float scale1Value, float scale1Min, float scale1Max, float scale2Min = 0, float scale2Max = 100)
        {
            return Utils.ConvertScales(scale1Value, scale1Min, scale1Max, scale2Min, scale2Max);
        }

        public static float SmoothDamp(this float number, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime);
        }
        public static float SmoothDamp(this float number, float target, ref float velocity, float smoothTime, float maxSpeed)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime, maxSpeed);
        }
        public static float SmoothDamp(this float number, float target, ref float velocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            return Mathf.SmoothDamp(number, target, ref velocity, smoothTime, maxSpeed, deltaTime);
        }
    }
}