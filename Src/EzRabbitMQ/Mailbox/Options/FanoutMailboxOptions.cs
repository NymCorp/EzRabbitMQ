using FluentValidation;

namespace EzRabbitMQ;

/// <summary>
/// Fanout mailbox options
/// </summary>
public record FanoutMailboxOptions : IMailboxOptions
{
    /// <summary>
    /// Fanout mailbox options
    /// </summary>
    /// <param name="queueName">Queue name.</param>
    /// <param name="exchangeName">Exchange name, fallbacks to "amq.fanout".</param>
    public FanoutMailboxOptions(string queueName, string? exchangeName = null)
    {
        QueueName = queueName;
        ExchangeName = exchangeName ?? ExchangeType.Fanout.Name();
    }

    /// <inheritdoc />
    public string ExchangeName { get; }

    /// <inheritdoc />
    public ExchangeType ExchangeType => ExchangeType.Fanout;

    /// <inheritdoc />
    public string RoutingKey => string.Empty;

    /// <inheritdoc />
    public string QueueName { get; }

    /// <inheritdoc />
    public string? CorrelationId => null;

    /// <inheritdoc />
    public Dictionary<string, string> QueueBindingHeaders { get; } = new();

    /// <inheritdoc />
    public Dictionary<string, string> SessionHeaders { get; } = new();
}

// ReSharper disable once UnusedType.Global
internal class FanoutMailboxOptionsValidator : AbstractValidator<FanoutMailboxOptions>
{
    public FanoutMailboxOptionsValidator()
    {
        RuleFor(m => m.QueueName)
            .NotNull()
            .NotEmpty()
            .WithMessage("Unable to create direct mailbox with null or empty queue name");
    }
}