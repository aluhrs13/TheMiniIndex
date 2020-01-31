using System;

namespace MiniIndex.Core.Utilities
{
    public static class StringExtensions
    {
        public static string AsNullIfEmpty(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            return value;
        }

        public static string AsNullIfWhiteSpaceOrEmpty(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }
    }
}