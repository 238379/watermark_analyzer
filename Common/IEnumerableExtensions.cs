using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public static class IEnumerableExtensions
    {
        public static async IAsyncEnumerable<T> ToIAsyncEnumerable<T>(this IEnumerable<T> items, [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach(var i in items)
            {
                yield return i;
            }
        }

        public static List<(T1, T2)> Combine<T1, T2>(this IEnumerable<T1> items, IEnumerable<T2> others)
        {
            var combined = new List<(T1, T2)>();

            foreach(var it in items)
            {
                foreach(var ot in others)
                {
                    combined.Add((it, ot));
                }
            }

            return combined;
        }
    }
}
