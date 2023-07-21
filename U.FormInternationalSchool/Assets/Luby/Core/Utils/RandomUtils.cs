using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LubyLib.Core
{
    public static class RandomUtils
    {
        /// <summary>
        ///     <para>Returns a random string.</para>
        /// </summary>
        /// <param name="length"></param>
        /// <param name="upperCaseLetters"></param>
        /// <param name="lowerCaseLetters"></param>
        /// <param name="numbers"></param>
        /// <param name="specialCharacters"></param>
        public static string String(int length = 5, bool upperCaseLetters = true, bool lowerCaseLetters = false, bool numbers = false, bool specialCharacters = false)
        {
            char[] chars = StringUtils.GetUpperCaseLettersArray();

            StringBuilder writer = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                writer.Append(FromArray(chars));
            }
            
            return writer.ToString();
        }

        /// <summary>
        ///     <para>Returns a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T FromArray<T>(T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
        /// <summary>
        ///     <para>Returns a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static T FromArray<T>(List<T> array)
        {
            return array[Random.Range(0, array.Count)];
        }

        /// <summary>
        ///     <para>Returns the index of a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static int ArrayIndex<T>(T[] array)
        {
            return Random.Range(0, array.Length);
        }
        /// <summary>
        ///     <para>Returns the index of a random element from the array</para>
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        public static int ArrayIndex<T>(List<T> array)
        {
            return Random.Range(0, array.Count);
        }
        
        /// <summary>
        /// Returns a random index, calculated by the weights.
        /// </summary>
        /// <param name="weights"></param>
        public static int IndexWeighted(int[] weights)
        {
            if (weights == null || weights.Length == 0) return -1;

            int total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
            }

            float rand = Random.value * total;
            for (int i = 0; i < weights.Length; i++)
            {
                var weight = weights[i];
                if (rand < weight)
                {
                    return i;
                }

                rand -= weight;
            }

            return 0;
        }
        /// <summary>
        /// Returns a random index, calculated by the weights.
        /// </summary>
        /// <param name="weights"></param>
        public static int IndexWeighted(List<int> weights)
        {
            if (weights == null || weights.Count == 0) return -1;

            int total = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                total += weights[i];
            }

            float rand = Random.value * total;
            for (int i = 0; i < weights.Count; i++)
            {
                var weight = weights[i];
                if (rand < weight)
                {
                    return i;
                }

                rand -= weight;
            }

            return 0;
        }
        /// <summary>
        /// Returns a random index, calculated by the weights.
        /// </summary>
        /// <param name="weights"></param>
        public static int IndexWeighted(float[] weights)
        {
            if (weights == null || weights.Length == 0) return -1;

            float total = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                total += weights[i];
            }

            float rand = Random.value * total;
            for (int i = 0; i < weights.Length; i++)
            {
                var weight = weights[i];
                if (rand < weight)
                {
                    return i;
                }

                rand -= weight;
            }

            return 0;
        }
        /// <summary>
        /// Returns a random index, calculated by the weights.
        /// </summary>
        /// <param name="weights"></param>
        public static float IndexWeighted(List<float> weights)
        {
            if (weights == null || weights.Count == 0) return -1;

            float total = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                total += weights[i];
            }

            float rand = Random.value * total;
            for (int i = 0; i < weights.Count; i++)
            {
                var weight = weights[i];
                if (rand < weight)
                {
                    return i;
                }

                rand -= weight;
            }

            return 0;
        }

        /// <summary>
        ///     <para>Returns true or false.</para>
        /// </summary>
        /// <param name="chanceOfTrue"></param>
        public static bool Bool(float chanceOfTrue = .5f)
        {
            return Random.Range(0f, 1f) <= chanceOfTrue;
        }

        /// <summary>
        ///     <para>Returns a random position within a rect.</para>
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 RandomPositionInRect(Rect rect)
        {
            return new Vector2(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
        }
        
        /// <summary>
        ///     <para>Returns a random number between min and max added to the base value.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="baseValue"></param>
        public static float Number(float min, float max, float baseValue)
        {
            return Number(min, max) + baseValue;
        }
        /// <summary>
        ///     <para>Returns a random number between min and max.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Number(float min, float max)
        {
            return Random.Range(min, max);
        }
        
        /// <summary>
        ///     <para>Returns a random number between min and max added to the base value.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="baseValue"></param>
        public static int Number(int min, int max, int baseValue)
        {
            return Number(min, max) + baseValue;
        }
        /// <summary>
        ///     <para>Returns a random number between min and max.</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Number(int min, int max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        ///     <para>Returns a number between -variation and variation added to the base value.</para>
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="baseValue"></param>
        public static float Variation(float variation, float baseValue)
        {
            return Number(-variation, variation, baseValue);
        }
        /// <summary>
        ///     <para>Returns a number between -variation and variation.</para>
        /// </summary>
        /// <param name="variation"></param>
        public static float Variation(float variation)
        {
            return Number(-variation, variation);
        }
        
        /// <summary>
        ///     <para>Returns a number between -variation and variation added to the base value.</para>
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="baseValue"></param>
        public static int Variation(int variation, int baseValue)
        {
            return Number(variation, baseValue);
        }
        /// <summary>
        ///     <para>Returns a number between -variation and variation.</para>
        /// </summary>
        /// <param name="variation"></param>
        public static int Variation(int variation)
        {
            return Number(-variation, variation);
        }

        #region Vector2

        /// <summary>
        ///     <para>Returns a random Vector2.</para>
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        public static Vector2 Vector2(float minX, float maxX, float minY, float maxY, float baseX, float baseY)
        {
            return new Vector2(Number(minX, maxX, baseX), Number(minY, maxY, baseY));
        }
        
        /// <summary>
        ///     <para>Returns a random Vector2.</para>
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        public static Vector2 Vector2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Number(minX, maxX), Number(minY, maxY));
        }
        
        /// <summary>
        ///     <para>Returns a random Vector2.</para>
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        public static Vector2Int Vector2(int minX, int maxX, int minY, int maxY, int baseX, int baseY)
        {
            return new Vector2Int(Number(minX, maxX, baseX), Number(minY, maxY, baseY));
        }
        /// <summary>
        ///     <para>Returns a random Vector2.</para>
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        public static Vector2Int Vector2(int minX, int maxX, int minY, int maxY)
        {
            return new Vector2Int(Number(minX, maxX), Number(minY, maxY));
        }
        
        /// <summary>
        ///     <para>Returns a Vector2 between variation and variation added to baseValue.</para>
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="baseValue"></param>
        public static Vector2 Vector2Variation(Vector2 variation, Vector2 baseValue)
        {
            return Vector2Variation(variation.x, variation.y, baseValue.x, baseValue.y);
        }

        /// <summary>
        ///     <para>Returns a Vector2 between -variation and variation.</para>
        /// </summary>
        /// <param name="variation"></param>
        public static Vector2 Vector2Variation(Vector2 variation)
        {
            return Vector2Variation(variation.x, variation.y);
        }
        
        /// <summary>
        ///     <para>Returns a Vector2 between variation and variation added to baseValue.</para>
        /// </summary>
        /// <param name="xVariation"></param>
        /// <param name="yVariation"></param>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        public static Vector2 Vector2Variation(float xVariation, float yVariation, float baseX, float baseY)
        {
            return Vector2(-xVariation, xVariation, -yVariation, yVariation, baseX, baseY);
        }
        
        /// <summary>
        ///     <para>Returns a Vector2 between -variation and variation.</para>
        /// </summary>
        /// <param name="xVariation"></param>
        /// <param name="yVariation"></param>
        public static Vector2 Vector2Variation(float xVariation, float yVariation)
        {
            return Vector2(-xVariation, xVariation, -yVariation, yVariation);
        }
        
        
        /// <summary>
        ///     <para>Returns a Vector2Int between variation and variation added to baseValue.</para>
        /// </summary>
        /// <param name="variation"></param>
        /// <param name="baseValue"></param>
        public static Vector2Int Vector2IntVariation(Vector2Int variation, Vector2Int baseValue)
        {
            return Vector2IntVariation(variation.x, variation.y, baseValue.x, baseValue.y);
        }
        
        /// <summary>
        ///     <para>Returns a Vector2Int between -variation and variation.</para>
        /// </summary>
        /// <param name="variation"></param>
        public static Vector2Int Vector2IntVariation(Vector2Int variation)
        {
            return Vector2IntVariation(variation.x, variation.y);
        }
        
        /// <summary>
        ///     <para>Returns a Vector2Int between variation and variation added to baseValue.</para>
        /// </summary>
        /// <param name="xVariation"></param>
        /// <param name="yVariation"></param>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        public static Vector2Int Vector2IntVariation(int xVariation, int yVariation, int baseX, int baseY)
        {
            return Vector2(-xVariation, xVariation, -yVariation, yVariation, baseX, baseY);
        }
        
        /// <summary>
        ///     <para>Returns a Vector2 between -variation and variation.</para>
        /// </summary>
        /// <param name="xVariation"></param>
        /// <param name="yVariation"></param>
        public static Vector2Int Vector2IntVariation(int xVariation, int yVariation)
        {
            return Vector2(-xVariation, xVariation, -yVariation, yVariation);
        }

        #endregion
    }
}
