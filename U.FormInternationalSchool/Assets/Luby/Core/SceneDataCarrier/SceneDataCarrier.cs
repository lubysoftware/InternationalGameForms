using System.Collections.Generic;
using LubyLib.Core.Singletons;

namespace LubyLib.Core
{
    /// <summary>
    ///     <para>Object that carries data through scenes.</para>
    /// </summary>
    public class SceneDataCarrier : SpawnableSingletonProtected<SceneDataCarrier>
    {
        protected override bool DestroyOnLoad => false;
        
        private Dictionary<int, object> _dataDictionary = new Dictionary<int, object>();
        
        /// <summary>
        ///     <para>Add data to be carried.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        private void _AddData<T>(string key, T data)
        {
            int hash = key.GetHashCode();
            if (_dataDictionary.ContainsKey(hash))
            {
                _dataDictionary[hash] = data;
            }
            else
            {
                _dataDictionary.Add(hash, data);
            }
        }
        
        /// <summary>
        ///     <para>Add data to be carried.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddData<T>(string key, T data)
        {
            Instance._AddData(key, data);
        }

        /// <summary>
        ///     <para>Retrieves data.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>If the key exists.</returns>
        private bool _GetData<T>(string key, out T data)
        {
            if (!_dataDictionary.TryGetValue(key.GetHashCode(), out var dicData))
            {
                data = default;
                return false;
            }

            data = (T) dicData;
            return true;
        }

        /// <summary>
        ///     <para>Retrieves data.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>If the key exists.</returns>
        public static bool GetData<T>(string key, out T data)
        {
            return Instance._GetData(key, out data);
        }

        /// <summary>
        ///     <para>Removes data from carrier.</para>
        /// </summary>
        /// <param name="key"></param>
        private void _RemoveData(string key)
        {
            _dataDictionary.Remove(key.GetHashCode());
        }

        /// <summary>
        ///     <para>Removes data from carrier.</para>
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveData(string key)
        {
            Instance._RemoveData(key);
        }
        
        /// <summary>
        ///     <para>Removes data from carrier and retrieves its value.</para>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveData<T>(string key, out T data)
        {
            Instance._GetData(key, out data);
            Instance._RemoveData(key);
        }

        /// <summary>
        ///     <para>Clear all data from carrier.</para>
        /// </summary>
        private void _ClearData()
        {
            _dataDictionary.Clear();
        }

        /// <summary>
        ///     <para>Clear all data from carrier.</para>
        /// </summary>
        public static void ClearData()
        {
            Instance._ClearData();
        }

    }
}
