using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniIndex.Core.Utilities
{
    public static class CharEnumerableExtensions
    {
        public static string AsString(this IEnumerable<char> source)
        {
            return new String(source.ToArray());
        }
    }
}