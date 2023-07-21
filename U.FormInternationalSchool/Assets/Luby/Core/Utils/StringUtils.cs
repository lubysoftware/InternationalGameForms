namespace LubyLib.Core
{
    public static class StringUtils
    {
        public static char[] GetUpperCaseLettersArray() => new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };
        
        public static char[] GetLowerCaseLettersArray() => new[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'v',
            'v', 'w', 'x', 'y', 'z'
        };
        public static char[] GetUpperAndLowerCaseLettersArray() =>  new[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'v',
            'v', 'w', 'x', 'y', 'z'
        };

        public static char[] GetNumbersArray() => new[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        

        public static string FirstLetterUppercase(this string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        
        public static string FirstLetterLowercase(this string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}