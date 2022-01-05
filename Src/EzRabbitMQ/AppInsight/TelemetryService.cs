using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace EzRabbitMQ;

/// <summary>
///     Application Insights Telemetry Client, implements <see cref="ITelemetryService" /> and <see cref="IDisposable" />.
/// </summary>
public class TelemetryService : ITelemetryService, IDisposable
{
    private readonly MailboxExtractInformation _mailboxTelemetryExtractor;
    private readonly ProducerExtractInformation _producerTelemetryExtractor;
    private readonly TelemetryClient _telemetryClient;

    /// <summary>
    /// Telemetry service.
    /// </summary>
    /// <param name="config">EzRabbitMQ configuration.</param>
    /// <param name="mailboxTelemetryExtractor">MailboxExtractor used to extract mailbox information.</param>
    /// <param name="producerTelemetryExtractor">ProducerExtractor used to extract producer information.</param>
    public TelemetryService(
        EzRabbitMQConfig config,
        MailboxExtractInformation mailboxTelemetryExtractor,
        ProducerExtractInformation producerTelemetryExtractor
    )
    {
        var telemetryConfig = new TelemetryConfiguration(config.InstrumentationKey);

        _telemetryClient = new TelemetryClient(telemetryConfig);

        _producerTelemetryExtractor = producerTelemetryExtractor;
        _mailboxTelemetryExtractor = mailboxTelemetryExtractor;

        _telemetryClient.TrackTrace("Telemetry client created");
    }

    /// <inheritdoc />
    public IOperationHolder<RequestTelemetry> Request(IMailboxOptions options, string operationName, Action<RequestTelemetry>? configure = default)
    {
        return InternalRequest(_mailboxTelemetryExtractor.CreateRequest(options, operationName), configure);
    }

    /// <inheritdoc />
    public IOperationHolder<DependencyTelemetry> Dependency(IProducerOptions options, string operationName, Action<DependencyTelemetry>? configure = default)
    {
        return InternalRequest(_producerTelemetryExtractor.CreateDependency(options, operationName), configure);
    }

    /// <inheritdoc />
    public IOperationHolder<DependencyTelemetry> Dependency(IMailboxOptions options, string operationName, Action<DependencyTelemetry>? configure = default)
    {
        return InternalRequest(_mailboxTelemetryExtractor.CreateDependency(options, operationName), configure);
    }

    /// <inheritdoc />
    public void Trace(string operationName, Dictionary<string, string>? props = default)
    {
        _telemetryClient.TrackTrace(operationName, props);
    }

    private IOperationHolder<T> InternalRequest<T>(T telemetry, Action<T>? configure = default) where T : OperationTelemetry, new()
    {
        var operationTelemetry = _telemetryClient.StartOperation(telemetry);

        configure?.Invoke(operationTelemetry.Telemetry);

        return operationTelemetry;
    }

    /// <summary>
    /// Flush telemetry client
    /// </summary>
    public void Dispose()
    {
        _telemetryClient.Flush();
    }
}