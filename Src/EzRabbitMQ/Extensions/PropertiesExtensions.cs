using Microsoft.ApplicationInsights.Extensibility.Implementation;
using RabbitMQ.Client;

namespace EzRabbitMQ.Extensions;

/// <summary>
/// Helpful methods to manipulate message's properties
/// </summary>
public static class PropertiesExtensions
{
    /// <summary>
    /// Initialize and set event's headers with operation id and operation parent id.
    /// </summary>
    /// <param name="properties">Properties of the message.</param>
    /// <param name="operation">Telemetry operation context.</param>
    public static void SetTelemetry(this IBasicProperties properties, OperationContext operation)
    {
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers[Constants.TelemetryOperationIdHeaderName] = operation.Id;
        properties.Headers[Constants.TelemetryParentOperationIdHeaderName] = operation.ParentId;
    }
}