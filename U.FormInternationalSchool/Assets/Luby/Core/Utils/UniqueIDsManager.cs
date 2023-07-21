using System;

namespace LubyLib.Core
{
    public static class UniqueIDsManager
    {
        /// <summary>
        ///     <para>Returns a unique id as a string</para>
        /// </summary>
        /// <param name="prefix"></param>
        public static string GetUniqueID(string prefix = "")
        {
            return $"{prefix}{Guid.NewGuid().ToString()}";
        }
    }
}
