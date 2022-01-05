namespace EzRabbitMQ.Extensions;

internal static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        Func<TValue> valueCreator) where TKey : notnull
    {
        return dict.TryGetValue(key, out var value) ? value : dict[key] = valueCreator();
    }
}