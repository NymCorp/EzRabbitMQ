using System;
using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// RPC mailbox options
    /// </summary>
    public record RpcClientMailboxOptions : IMailboxOptions
    {
        /// <summary>
        /// Create a client with a specific queue name
        /// </summary>
        /// <param name="routingKey">Routing key, must match with the RPC server name</param>
        public RpcClientMailboxOptions(string? routingKey = null)
        {
            RoutingKey = routingKey ?? Constants.RpcDefaultRoutingKey;
        }
        
        /// <inheritdoc />
        public string ExchangeName => string.Empty;

        /// <inheritdoc />
        public ExchangeType ExchangeType => ExchangeType.RpcClient;

        /// <inheritdoc />
        public string RoutingKey { get; }

        /// <inheritdoc />
        public string QueueName => Constants.RpcReplyToQueue;

        /// <inheritdoc />
        public string CorrelationId { get; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        public Dictionary<string, string> QueueBindingHeaders { get; } = new();

        /// <inheritdoc />
        public Dictionary<string, string> SessionHeaders { get; } = new();
    }
}