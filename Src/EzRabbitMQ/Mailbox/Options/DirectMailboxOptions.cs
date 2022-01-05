using FluentValidation;

namespace EzRabbitMQ;

/// <summary>
/// Direct mailbox Options
/// </summary>
public record DirectMailboxOptions : IMailboxOptions
{
    /// <summary>
    /// Direct mailbox options
    /// </summary>
    /// <param name="routingKey">Routing key e.g.: integration</param>
    /// <param name="queueName">Queue name</param>
    /// <param name="exchangeName">Routing key</param>
    public DirectMailboxOptions(string routingKey, string queueName, string? exchangeName = null)
    {
        RoutingKey = routingKey;
        QueueName = queueName;
        ExchangeName = exchangeName ?? ExchangeType.Direct.Name();
    }

    /// <inheritdoc />
    public string ExchangeName { get; }

    /// <inheritdoc />
    public ExchangeType ExchangeType => ExchangeType.Direct;

    /// <inheritdoc />
    public string RoutingKey { get; }

    /// <inheritdoc />
    public string QueueName { get; }

    /// <inheritdoc />
    public string CorrelationId => null!;

    /// <inheritdoc />
    public Dictionary<string, string> QueueBindingHeaders { get; } = new();

    /// <inheritdoc />
    public Dictionary<string, string> SessionHeaders { get; } = new();
}

// ReSharper disable once UnusedType.Global
internal class DirectMailboxOptionsValidator : AbstractValidator<DirectMailboxOptions>
{
    public DirectMailboxOptionsValidator()
    {
        RuleFor(m => m.QueueName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Unable to create direct mailbox with null or empty queue name");

        RuleFor(m => m.RoutingKey)
            .NotNull()
            .NotEmpty()
            .WithMessage("Unable to create direct mailbox with null or empty routing key");
    }
}