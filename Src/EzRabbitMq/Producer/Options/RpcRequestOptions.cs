namespace EzRabbitMQ
{
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
}