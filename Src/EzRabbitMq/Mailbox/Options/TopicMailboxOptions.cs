
using System.Collections.Generic;
using FluentValidation;

namespace EzRabbitMQ
{
    /// <summary>
    /// Topic mailbox options
    /// </summary>
    public record TopicMailboxOptions : IMailboxOptions
    {
        /// <summary>
        /// Topic mailbox options
        /// </summary>
        /// <param name="routingKey">Topic routing key, must be word split by dots with # or *</param>
        /// <param name="queueName">Queue name</param>
        /// <param name="exchangeName">Exchange name</param>
        public TopicMailboxOptions(string routingKey, string queueName, string? exchangeName = null)
        {
            RoutingKey = routingKey;
            QueueName = queueName;
            ExchangeName = exchangeName ?? ExchangeType.Name();
        }

        /// <inheritdoc />
        public string ExchangeName { get; }

        /// <inheritdoc />
        public ExchangeType ExchangeType => ExchangeType.Topic;

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
    internal class TopicMailboxOptionsValidator : AbstractValidator<TopicMailboxOptions>
    {
        /// <summary>
        /// Hash can replace one or more values
        /// </summary>
        public const char Hash = '#';
        
        /// <summary>
        /// Star can only replace one word
        /// </summary>
        public const char Star = '*';
        
        public TopicMailboxOptionsValidator()
        {
            RuleFor(m => m.QueueName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Unable to create direct mailbox with null or empty queue name");
            
            RuleFor(m => m.RoutingKey)
                .NotNull()
                .NotEmpty()
                .Must(routingKey => routingKey.Contains('.'))
                .Must(routingKey => routingKey.Contains(Hash) || routingKey.Contains(Star))
                .WithMessage("Invalid routing key for topic, " +
                             "the routing key must be lower case word split by dots, " +
                             "and must contain a '*' (can substitute for exactly one word) " +
                             "or a '#' (can substitute for zero or more words)." +
                             "E.g.: root.a.#");
        }
    }
}