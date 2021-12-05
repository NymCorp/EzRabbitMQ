using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// Contains the mailbox options
    /// </summary>
    public interface IMailboxOptions
    {
        /// <summary>
        /// Exchange name
        /// </summary>
        public string? ExchangeName { get; }

        /// <summary>
        /// Exchange type see : <see cref="EzRabbitMQ.ExchangeType"/>
        /// </summary>
        public ExchangeType ExchangeType { get; }

        /// <summary>
        /// Routing key
        /// </summary>
        public string? RoutingKey { get; }

        /// <summary>
        /// Queue name
        /// </summary>
        public string QueueName { get; }

        /// <summary>
        /// CorrelationId for RPC call
        /// </summary>
        public string? CorrelationId { get; }

        /// <summary>
        /// Queue binding headers
        /// </summary>
        public Dictionary<string, string> QueueBindingHeaders { get; }
        
        /// <summary>
        /// Session headers
        /// </summary>
        public Dictionary<string, string> SessionHeaders { get; }
    }
}