using Microsoft.ApplicationInsights.DataContracts;

namespace EzRabbitMQ;

/// <summary>
/// Contract for telemetry extractors
/// </summary>
/// <typeparam name="T">type of the date you want to extract</typeparam>
public abstract class ExtractTelemetryInformationBase<T>
{
    /// <summary>
    /// Extract information from data to set request properties
    /// </summary>
    /// <param name="data">Contextual data</param>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <returns><see cref="RequestTelemetry"/></returns>
    public abstract RequestTelemetry CreateRequest(T data, string operationName);

    /// <summary>
    /// Extract information from data to set dependency properties
    /// </summary>
    /// <param name="data">Contextual data</param>
    /// <param name="operationName">Operation name, this will be the label associated to your request</param>
    /// <returns><see cref="DependencyTelemetry"/></returns>
    public abstract DependencyTelemetry CreateDependency(T data, string operationName);

    /// <summary>
    /// Set the properties depending on the type T
    /// </summary>
    /// <param name="data">Contextual data</param>
    /// <param name="properties">Properties or request or dependency operation</param>
    protected abstract void PopulateProperties(T data, IDictionary<string, string> properties);
}