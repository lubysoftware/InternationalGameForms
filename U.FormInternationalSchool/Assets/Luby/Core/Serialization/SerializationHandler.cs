using UnityEngine;

namespace LubyLib.Core.Serialization
{
    public static class SerializationHandler
    {
        /// <summary>
        ///     <para>Serializes an object to JSON</para>
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static string Serialize<T>(T obj)
        {
	        return JsonUtility.ToJson(obj);
        }
        
        /// <summary>
        ///     <para>Deserializes an object from JSON</para>
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        public static T Deserialize<T>(string json)
        {
	        return JsonUtility.FromJson<T>(json);
        }
    }
}