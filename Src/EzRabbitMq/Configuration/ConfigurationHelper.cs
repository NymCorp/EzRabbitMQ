using System;
using Microsoft.Extensions.Configuration;

namespace EzRabbitMQ
{
    /// <summary>
    /// Pre registered configuration key
    /// </summary>
    public record ConfigurationKeys
    {
        /// <summary>
        /// Logging LogLevel for EzRabbitMQ
        /// </summary>
        public const string LogLevelKey = "Logging:LogLevel:EzRabbitMQ";

        /// <summary>
        /// Application Insights instrumentation key
        /// </summary>
        public const string AppInsightKey = "ApplicationInsights:InstrumentationKey";
    }

    /// <summary>
    /// Configuration utils extensions method
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Try get a value in configuration
        /// </summary>
        /// <param name="config">Configuration to read</param>
        /// <param name="key">Key to fetch from configuration</param>
        /// <param name="value">Value found returned</param>
        /// <returns>True if found, false if not</returns>
        public static bool TryGet(this IConfiguration config, string key, out string value)
        {
            return (value = config[key]) is not null;
        }

        /// <summary>
        /// Try to read an enum value from configuration
        /// </summary>
        /// <param name="config">Configuration to read</param>
        /// <param name="key">Key to fetch from configuration</param>
        /// <param name="value">Enum value found in configuration</param>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <returns>True if found</returns>
        public static bool TryGetEnum<TEnum>(this IConfiguration config, string key, out TEnum value) where TEnum : struct
        {
            var valueStr = config[key];
            
            value = default;
            
            if (valueStr is null) return false;

            return Enum.TryParse(valueStr, out value);
        }
    }
}