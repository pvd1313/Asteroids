using System.Collections.Generic;

namespace Asteroids
{
    public static class ListExtensions
    {
        public static bool AddIfNotExists<T>(this List<T> list, T item)
        {
            if (list.Contains(item)) return false;
            list.Add(item);
            return true;
        }
    }
}
