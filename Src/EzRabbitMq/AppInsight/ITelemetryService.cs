using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace EzRabbitMQ;

/// <summary>
/// Telemetry service used to send metrics and trace to appInsight
/// </summary>
public interface ITelemetryService
{
    /// <summary>
    /// Create a request based on mailboxes data
    /// </summary>
    /// <param name="options">Mailbox option</param>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <param name="configure">Configure request</param>
    /// <returns>Operation holder</returns>
    IOperationHolder<RequestTelemetry> Request(IMailboxOptions options, string operationName, Action<RequestTelemetry>? configure = default);

    /// <summary>
    /// Create a dependency request based on producer data
    /// </summary>
    /// <param name="options">Mailbox options</param>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <param name="configure">Configure action allow to manipulate the dependency telemetry</param>
    /// <returns>Operation holder</returns>
    IOperationHolder<DependencyTelemetry> Dependency(IProducerOptions options, string operationName, Action<DependencyTelemetry>? configure = default);

    /// <summary>
    /// Create a dependency request base on mailbox options
    /// </summary>
    /// <param name="options">Mailbox options</param>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <param name="configure">Configure action allow to manipulate the dependency telemetry</param>
    /// <returns>Operation holder</returns>
    IOperationHolder<DependencyTelemetry> Dependency(IMailboxOptions options, string operationName, Action<DependencyTelemetry>? configure = default);

    /// <summary>
    /// Simple trace operation
    /// </summary>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <param name="props">Set custom properties to the trace operation</param>
    void Trace(string operationName, Dictionary<string, string>? props = default);
}