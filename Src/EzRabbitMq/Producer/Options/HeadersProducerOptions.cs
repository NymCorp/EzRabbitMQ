using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// Headers producer options
    /// </summary>
    public record HeadersProducerOptions : IProducerOptions
    {
        /// <summary>
        /// Headers producer options
        /// </summary>
        /// <param name="headers">Header of the message, used for routing message</param>
        /// <param name="exchangeName">Exchange name, fallback on default exchange name</param>
        public HeadersProducerOptions(Dictionary<string, string> headers, string? exchangeName = null)
        {
            RoutingKey = string.Empty;
            
            ExchangeName = exchangeName ?? ExchangeType.Headers.Name();

            Properties.Headers = headers;
        }

        /// <inheritdoc />
        public string RoutingKey { get; }

        /// <inheritdoc />
        public string ExchangeName { get; }

        /// <inheritdoc />
        public ProducerProperties Properties { get; } = new ();
    }
}