using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsotericDevZone.Celesta.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static void ForEach<T, __Discarded>(this IEnumerable<T> items, Func<T, __Discarded> action)
        {
            foreach (var item in items)
                _ = action(item);
        }

        public static string JoinToString<T>(this IEnumerable<T> items, string separator = "\n")
            => string.Join(separator, items.Select(_ => _?.ToString() ?? "<null>"));

        public static IEnumerable<T> Peek<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
                yield return item;
            }
            yield break;
        }

        public static IEnumerable<T> Peek<T, _>(this IEnumerable<T> items, Func<T, _> action)
        {
            foreach (var item in items)
            {
                action(item);
                yield return item;
            }
            yield break;
        }



    }
}
