using UnityEngine;

namespace LubyLib.Core.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        ///     <para>Returns the Angle of vector</para>
        /// </summary>
        /// <param name="vector"></param>
        public static float Angle(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// <para>Returns the clockwise perpendicular of vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        public static Vector2 PerpendicularClockwise(this Vector2 vector)
        {
            return new Vector2(vector.y, -vector.x);
        }
        /// <summary>
        ///     <para>Returns the counter clockwise perpendicular of vector.</para>
        /// </summary>
        /// <param name="vector"></param>
        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector)
        {
            return new Vector2(-vector.x, vector.y);
        }
    }
}