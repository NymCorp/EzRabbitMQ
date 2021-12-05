using System;
using System.Collections.Generic;

namespace EzRabbitMQ.Extensions
{
    internal static class DictionnaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, 
            Func<TValue> valueCreator)
        {
            return dict.TryGetValue(key, out var value) ? value : dict[key] = valueCreator();
        }
    }
}