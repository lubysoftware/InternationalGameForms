using System.Collections.Generic;

namespace LubyLib.Core.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        ///     <para>Returns a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T GetRandomElement<T>(T[] array)
        {
            return RandomUtils.FromArray(array);
        }
        /// <summary>
        ///     <para>Returns a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T GetRandomElement<T>(this List<T> array)
        {
            return RandomUtils.FromArray(array);
        }
        /// <summary>
        ///     <para>Returns the index of a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static int GetRandomIndex<T>(T[] array)
        {
            return RandomUtils.ArrayIndex(array);
        }
        /// <summary>
        ///     <para>Returns the index of a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static int GetRandomIndex<T>(this List<T> array)
        {
            return RandomUtils.ArrayIndex(array);
        }
        
    }
}