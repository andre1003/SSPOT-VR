using System;
using System.Collections.Generic;

namespace SSpot.Utilities
{
    public static class ReadOnlyListUtilities
    {
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(item))
                    return i;
            }
            return -1;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> match) => 
            FindIndex(list, 0, match);
        
        public static int FindIndex<T>(this IReadOnlyList<T> list, int startIndex, Predicate<T> match)
        {
            for (int i = startIndex; i < list.Count; i++)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }
        
        public static int LastIndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Equals(item))
                    return i;
            }
            return -1;
        }
        
        public static int FindLastIndex<T>(this IReadOnlyList<T> list, Predicate<T> match)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (match(list[i]))
                    return i;
            }
            return -1;
        }
    }
}