using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AsyncProgramming.Definitions
{
    internal class AsyncCache<TKey, TValue>
    {
        private readonly Func<TKey, Task<TValue>> _valueFactory;
        private readonly ConcurrentDictionary<TKey, Lazy<Task<TValue>>> _map;

        public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _map = new ConcurrentDictionary<TKey, Lazy<Task<TValue>>>();
        }

        public Task<TValue> this[TKey key]
        {
            get
            {
                key = key ?? throw new ArgumentNullException(nameof(key));
                return _map.GetOrAdd(key, toAdd => new Lazy<Task<TValue>>(() => _valueFactory(toAdd))).Value;
            }
        }
    }
}
