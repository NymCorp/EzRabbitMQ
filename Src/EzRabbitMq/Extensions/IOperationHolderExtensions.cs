using System;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace EzRabbitMQ.Extensions
{
    /// <summary>
    /// Contains extensions method on <see cref="IOperationHolder{T}"/>
    /// </summary>
    public static class OperationHolderExtensions
    {
        /// <summary>
        /// Set the exception in request property and set fail status code and success false
        /// </summary>
        /// <param name="operation">operation holder</param>
        /// <param name="ex">exception triggered</param>
        /// <typeparam name="T">Operation type</typeparam>
        public static void Exception<T>(this IOperationHolder<T> operation, Exception ex) where T : OperationTelemetry, new()
        {
            if (operation.Telemetry is RequestTelemetry requestTelemetry)
                requestTelemetry.ResponseCode = "500";

            operation.Telemetry.Success = false;
            operation.Telemetry.Properties.Add("exception", ex.ToString());
            
            operation.Success(false);
        }

        /// <summary>
        /// Set success status of an operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="success"></param>
        /// <typeparam name="T"></typeparam>
        private static void Success<T>(this IOperationHolder<T> operation, bool success) where T : OperationTelemetry, new()
        {
            if (operation.Telemetry is RequestTelemetry requestTelemetry)
            {
                requestTelemetry.ResponseCode = success ? "200" : "500";
            }

            operation.Telemetry.Success = success;
        }
    }
}