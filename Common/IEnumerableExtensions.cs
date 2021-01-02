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
    }
}
