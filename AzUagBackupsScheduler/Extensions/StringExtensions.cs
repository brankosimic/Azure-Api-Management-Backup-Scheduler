using System.Globalization;

namespace AzUagBackupsScheduler
{
    public static class StringExtensions
    {
        public static string Format(this string text, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, text, args);
        }
    }
}
