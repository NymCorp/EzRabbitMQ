namespace EzRabbitMQ.Reflection;

/// <summary>
/// Cache used to optimize message serialization/deserialization and handle discovery
/// </summary>
public static class CachedReflection
{
    private static readonly Lazy<Dictionary<string, MethodInfo>> HandleCache = new(() => new());
    private static readonly Lazy<Dictionary<string, Type>> TypeCache = new(() => new());

    /// <summary>
    /// Find handle matching parameters filters and store the methodInfo in cache
    /// to improve performance.
    /// </summary>
    /// <param name="realImp">Type scanned to find method matching parameter type.</param>
    /// <param name="paramTypeText">Parameter type used to find method to call.</param>
    /// <param name="handleName">Name of the method you want to call.</param>
    /// <returns>MethodInfo matching parameters.</returns>
    public static MethodInfo? FindMethodToInvoke(Type realImp, string paramTypeText, string handleName)
    {
        var key = $"{realImp.Name}-{paramTypeText}-{handleName}";

        if (HandleCache.Value.TryGetValue(key, out var handle)) return handle;

        var paramType = GetType(paramTypeText);

        if (paramType is null) return null;

        var method = realImp.FindMatchingMethod(handleName, paramType);

        if (method is null) return null;

        HandleCache.Value[key] = method;

        return method;
    }

    /// <summary>
    /// Call GetType and cache the value.
    /// </summary>
    /// <param name="typeAssemblyQualifiedName">the assembly qualified of the type.</param>
    /// <returns>Nullable found type.</returns>
    public static Type? GetType(string typeAssemblyQualifiedName)
    {
        if (TypeCache.Value.TryGetValue(typeAssemblyQualifiedName, out var cachedType)) return cachedType;

        var type = Type.GetType(typeAssemblyQualifiedName);

        if (type is null) return null;

        TypeCache.Value[typeAssemblyQualifiedName] = type;

        return type;
    }
}