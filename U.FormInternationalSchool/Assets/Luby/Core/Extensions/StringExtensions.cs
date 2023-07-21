namespace LubyLib.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     <para>Returns if the string is null, empty or contains only space characters.</para>
        /// </summary>
        /// <param name="value"></param>
        public static bool IsNullEmptyOrWhitespace(this string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }
    }
}