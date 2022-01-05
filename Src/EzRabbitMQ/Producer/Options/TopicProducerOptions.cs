using FluentValidation;

namespace EzRabbitMQ;

/// <summary>
/// Topic producer options
/// </summary>
public record TopicProducerOptions : IProducerOptions
{
    /// <summary>
    /// Topic producer options
    /// </summary>
    /// <param name="routingKey">Topic routing key</param>
    /// <param name="exchangeName">Override default exchange name</param>
    public TopicProducerOptions(string routingKey, string? exchangeName = null)
    {
        RoutingKey = routingKey;
        ExchangeName = exchangeName ?? ExchangeType.Topic.Name();
    }

    /// <inheritdoc />
    public string RoutingKey { get; }

    /// <inheritdoc />
    public string ExchangeName { get; }

    /// <inheritdoc />
    public ProducerProperties Properties { get; } = new();
}

// ReSharper disable once UnusedType.Global
internal class TopicProducerOptionsValidator : AbstractValidator<TopicProducerOptions>
{
    public TopicProducerOptionsValidator()
    {
        RuleFor(m => m.RoutingKey)
            .NotNull()
            .NotEmpty()
            .Must(o => o.Split(".").Length > 1)
            .WithMessage("Unable to create topic producer with null or empty queue name, or containing only one world");
    }
}