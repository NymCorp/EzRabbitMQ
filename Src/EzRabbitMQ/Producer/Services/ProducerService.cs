using Microsoft.Extensions.DependencyInjection;

namespace EzRabbitMQ;

/// <inheritdoc />
public class ProducerService : IProducerService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ValidationService _validationService;

    /// <summary>
    ///  Producer Service, this service is used to create messages.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    /// <param name="validationService">Validation service used to validate options.</param>
    public ProducerService(IServiceProvider serviceProvider, ValidationService validationService)
    {
        _serviceProvider = serviceProvider;
        _validationService = validationService;
    }

    /// <inheritdoc />
    public Producer Create(IProducerOptions options)
    {
        return ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);
    }

    /// <inheritdoc />
    public void Send<T>(IProducerOptions options, T data)
    {
        _validationService.ValidateAndThrow(options);

        using var producer = ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);

        producer.Publish(data);
    }

    /// <inheritdoc />
    public Producer Direct(string routingKey, string? exchangeName = null!)
    {
        var options = new DirectProducerOptions(routingKey, exchangeName);

        _validationService.ValidateAndThrow(options);

        return ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);
    }

    /// <inheritdoc />
    public void DirectSend<T>(string routingKey, T data, string? exchangeName = null!)
    {
        using var producer = Direct(routingKey, exchangeName);
        producer.Publish(data);
    }

    /// <inheritdoc />
    public Producer Topic(string routingKey, string? exchangeName = null!)
    {
        var options = new TopicProducerOptions(routingKey, exchangeName);

        _validationService.ValidateAndThrow(options);

        return ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);
    }

    /// <inheritdoc />
    public void TopicSend<T>(string routingKey, T data, string? exchangeName = null!)
    {
        using var producer = Topic(routingKey, exchangeName);
        producer.Publish(data);
    }

    /// <inheritdoc />
    public Producer Headers(Dictionary<string, string> headers, string? exchangeName = null!)
    {
        var options = new HeadersProducerOptions(headers, exchangeName);

        _validationService.ValidateAndThrow(options);

        return ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);
    }

    /// <inheritdoc />
    public void HeadersSend<T>(Dictionary<string, string> headers, T data, string? exchangeName = null!)
    {
        using var producer = Headers(headers, exchangeName);
        producer.Publish(data);
    }

    /// <inheritdoc />
    public Producer Fanout(string? exchangeName = null!)
    {
        var options = new FanoutProducerOptions(exchangeName);

        _validationService.ValidateAndThrow(options);

        return ActivatorUtilities.CreateInstance<Producer>(_serviceProvider, options);
    }

    /// <inheritdoc />
    public void FanoutSend<T>(T data, string? exchangeName = null!)
    {
        using var producer = Fanout(exchangeName);
        producer.Publish(data);
    }
}