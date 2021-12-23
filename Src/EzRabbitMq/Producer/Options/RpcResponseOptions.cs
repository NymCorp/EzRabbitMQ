using FluentValidation;

namespace EzRabbitMQ;

/// <summary>
/// RPC producer options
/// </summary>
public record RpcResponseOptions : IProducerOptions
{
    /// <summary>
    /// Rpc response options
    /// </summary>
    /// <param name="routingKey">Client address.</param>
    /// <param name="correlationId">Set the correlationId for the RPC response.</param>
    public RpcResponseOptions(string routingKey, string correlationId)
    {
        RoutingKey = routingKey;
        Properties.ReplyTo = Constants.RpcReplyToQueue;
        Properties.CorrelationId = correlationId;
    }

    /// <inheritdoc />
    public string RoutingKey { get; } = Constants.RpcDefaultRoutingKey;

    /// <inheritdoc />
    public string ExchangeName => string.Empty;

    /// <inheritdoc />
    public ProducerProperties Properties { get; } = new();
}

// ReSharper disable once UnusedType.Global
internal class RpcResponseOptionsValidator : AbstractValidator<RpcResponseOptions>
{
    public RpcResponseOptionsValidator()
    {
        RuleFor(x => x.RoutingKey).NotNull().NotEmpty()
            .WithMessage("RpcResponse needs a valid routing key");
        RuleFor(x => x.Properties.CorrelationId).NotNull().NotEmpty()
            .WithMessage("RpcResponse needs a valid correlation id");
    }
}