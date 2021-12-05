using System.Collections.Generic;
using FluentValidation;

namespace EzRabbitMQ
{
    /// <summary>
    /// Headers mailbox options
    /// </summary>
    public record HeadersMailboxOptions : IMailboxOptions
    {
        /// <summary>
        /// Header type mailbox options
        /// </summary>
        /// <param name="headers">Headers : e.g.: format: excel</param>
        /// <param name="xMatch">Match type, all or any</param>
        /// <param name="queueName">Queue name</param>
        /// <param name="exchangeName">Exchange name</param>
        public HeadersMailboxOptions(Dictionary<string, string> headers, XMatch xMatch, string queueName, string? exchangeName = null)
        {
            SessionHeaders = headers;

            QueueBindingHeaders = headers;
            
            QueueBindingHeaders.Add(Constants.XMatchHeaderKey, xMatch.GetTextValue());

            RoutingKey = string.Empty;

            QueueName = queueName;

            ExchangeName = exchangeName ?? ExchangeType.Headers.Name();
        }

        /// <inheritdoc />
        public virtual string ExchangeName { get; }

        /// <inheritdoc />
        public ExchangeType ExchangeType => ExchangeType.Headers;

        /// <inheritdoc />
        public virtual string RoutingKey { get; }

        /// <inheritdoc />
        public virtual string QueueName { get; }

        /// <inheritdoc />
        public string? CorrelationId => null;

        /// <inheritdoc />
        public Dictionary<string, string> QueueBindingHeaders { get; } = new();

        /// <inheritdoc />
        public Dictionary<string, string> SessionHeaders { get; } = new();
    }
    
    // ReSharper disable once UnusedType.Global
    internal class HeadersMailboxOptionsValidator : AbstractValidator<HeadersMailboxOptions>
    {
        public HeadersMailboxOptionsValidator()
        {
            RuleFor(m => m.QueueName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Unable to create direct mailbox with null or empty queue name");
            
            RuleFor(m => m.SessionHeaders)
                .NotNull()
                .NotEmpty()
                .WithMessage("Unable to create headers mailbox without any headers");
        }
    }
}