using System;
using System.Collections.Generic;

namespace SSpot.Utilities
{
    public static class EnumerableUtilities
    {
        public static void ForEach<T>(this IEnumerable<T> col, Action<T, int> action)
        {
            int i = 0;
            foreach (var x in col)
            {
                action(x, i);
                i++;
            }
        }
        
        public static void ForEach<T>(this IEnumerable<T> col, Action<T> action)
        {
            foreach (var x in col)
            {
                action(x);
            }
        }
    }
}