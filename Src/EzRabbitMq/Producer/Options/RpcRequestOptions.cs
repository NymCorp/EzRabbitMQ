using FluentValidation;

namespace EzRabbitMQ;

/// <summary>
/// RPC producer request options
/// </summary>
public record RpcRequestOptions : IProducerOptions
{
    /// <summary>
    /// Create a rpc request to server
    /// </summary>
    /// <param name="correlationId">Client generated Id used by the server to send back the data</param>
    public RpcRequestOptions(string correlationId)
    {
        Properties.ReplyTo = Constants.RpcReplyToQueue;

        Properties.CorrelationId = correlationId;

        Properties.DeliveryMode = DeliveryMode.NonPersistent;
    }

    /// <inheritdoc />
    public string RoutingKey => string.Empty;

    /// <inheritdoc />
    public string ExchangeName => string.Empty;

    /// <inheritdoc />
    public ProducerProperties Properties { get; } = new();
}

// ReSharper disable once UnusedType.Global
internal class RpcRequestOptionsValidator : AbstractValidator<RpcRequestOptions>
{
    public RpcRequestOptionsValidator()
    {
        RuleFor(x => x.Properties.CorrelationId).NotNull().NotEmpty()
            .WithMessage("Unable to send a RPC request without a correlation id");
    }
}