using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EzRabbitMQ.Reflection
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<MethodInfo> FindMethods(this Type type, string methodName)
        {
            return type.GetMethods().Where(m => m.Name == methodName);
        }

        public static Type? GetFirstParamType(this MethodInfo method)
        {
            return method.GetParameters().FirstOrDefault()?.ParameterType;
        }

        public static Type? GetFirstArgumentType(this Type type)
        {
            return type.GetGenericArguments().FirstOrDefault();
        }

        public static MethodInfo? FindMatchingMethod(this Type type, string methodName, Type messageType)
        {
            var methods = type.FindMethods(methodName);

            return (from method in methods
                    let paramType = method.GetFirstParamType()
                    let argType = paramType?.GetFirstArgumentType()
                    let priority = messageType.IsAssignableTo(argType) ? 0 : 1
                    where paramType == messageType
                          || argType == messageType
                          || messageType.IsAssignableTo(argType)
                    select (priority, method))
                .OrderBy(m => m.priority)
                .Select(m => m.method)
                .FirstOrDefault();
        }
    }
}