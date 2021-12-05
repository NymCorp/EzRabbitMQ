using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// RPC mailbox options
    /// </summary>
    public record RpcServerMailboxOptions : IMailboxOptions
    {
        /// <summary>
        /// Rpc Server options
        /// </summary>
        public RpcServerMailboxOptions(string? queueName = null)
        {
            QueueName = queueName ?? Constants.RpcDefaultRoutingKey;
        }

        /// <inheritdoc />
        public string ExchangeName => ExchangeType.RpcServer.Name();

        /// <inheritdoc />
        public ExchangeType ExchangeType => ExchangeType.RpcServer;

        /// <inheritdoc />
        public string RoutingKey => QueueName;

        /// <inheritdoc />
        public string QueueName { get; }

        /// <inheritdoc />
        public string CorrelationId => string.Empty;

        /// <inheritdoc />
        public Dictionary<string, string> QueueBindingHeaders { get; } = new();

        /// <inheritdoc />
        public Dictionary<string, string> SessionHeaders { get; } = new();
    }
}