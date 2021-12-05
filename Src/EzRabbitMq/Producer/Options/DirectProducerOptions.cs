namespace EzRabbitMQ
{
    /// <summary>
    /// Direct producer options
    /// </summary>
    public record DirectProducerOptions : IProducerOptions
    {
        /// <summary>
        /// Direct producer options
        /// </summary>
        /// <param name="routingKey">Direct type routing key</param>
        /// <param name="exchangeName">Exchange name, fallback on default exchange name</param>
        public DirectProducerOptions(string routingKey, string? exchangeName = null)
        {
            RoutingKey = routingKey;
            ExchangeName = exchangeName ?? ExchangeType.Direct.Name();
        }

        /// <inheritdoc />
        public string RoutingKey { get; }

        /// <inheritdoc />
        public string ExchangeName { get; }

        /// <inheritdoc />
        public ProducerProperties Properties { get; } = new();
    }
}