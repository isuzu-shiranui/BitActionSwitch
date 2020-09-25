using System.IO;

namespace BitActionSwitch.Editor.Utility
{
    public static class StringUtil
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();
        
        public static bool HasInvalidChars(this string text)
        {
            return string.IsNullOrWhiteSpace(text) || text.IndexOfAny(InvalidChars) >= 0;
        }
    }
}