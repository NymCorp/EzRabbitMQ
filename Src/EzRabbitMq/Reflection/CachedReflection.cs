using System;
using System.Collections.Generic;
using System.Reflection;
using EzRabbitMQ.Exceptions;

namespace EzRabbitMQ.Reflection
{
    /// <summary>
    /// Cache used to optimize message serialization/deserialization and handle discovery
    /// </summary>
    public static class CachedReflection
    {
        private static readonly Lazy<Dictionary<string, MethodInfo>> HandleCache = new(() => new());
        private static readonly Lazy<Dictionary<string, Type>> TypeCache = new(() => new());

        /// <summary>
        /// Find handle matching parameters filters and store the methodInfo in cache
        /// to improve performance
        /// </summary>
        /// <param name="realImp">Type scanned to find method matching parameter type</param>
        /// <param name="paramTypeText">Parameter type used to find method to call</param>
        /// <param name="handleName">Name of the method you want to call</param>
        /// <returns>MethodInfo matching parameters</returns>
        /// <exception cref="ReflectionNotFoundTypeException"></exception>
        /// <exception cref="ReflectionNotFoundHandleException"></exception>
        public static MethodInfo FindMethodToInvoke(Type realImp, string paramTypeText, string handleName)
        {
            var key = $"{realImp.Name}-{paramTypeText}";
            
            if (HandleCache.Value.TryGetValue(key, out var handle)) return handle;
            
            var paramType = GetType(paramTypeText);

            var method = realImp.FindMatchingMethod(handleName, paramType);

            if (method is null) throw new ReflectionNotFoundHandleException(realImp.Name, handleName, paramTypeText);

            HandleCache.Value[key] = method;

            return method;
        }

        /// <summary>
        /// Call GetType and cache the value
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">the assembly qualified of the type</param>
        /// <returns>Found Type</returns>
        /// <exception cref="ReflectionNotFoundTypeException"></exception>
        public static Type GetType(string typeAssemblyQualifiedName)
        {
            if (TypeCache.Value.TryGetValue(typeAssemblyQualifiedName, out var cachedType)) return cachedType;

            var type = Type.GetType(typeAssemblyQualifiedName);

            if (type is null) throw new ReflectionNotFoundTypeException(typeAssemblyQualifiedName);

            TypeCache.Value[typeAssemblyQualifiedName] = type;

            return type;
        }
    }
}