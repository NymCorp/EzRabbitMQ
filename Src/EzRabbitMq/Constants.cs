namespace EzRabbitMQ
{
    /// <summary>
    /// Constants 
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// XMatch argument used for headers
        /// </summary>
        public const string XMatchHeaderKey = "x-match";

        // Queue predefined keys
        
        /// <summary>
        /// Queue max length argument name
        /// </summary>
        public const string QueueArgsMaxLength = "x-max-length";

        /// <summary>
        /// Queue mode argument name
        /// </summary>
        public const string QueueArgsMode = "x-queue-mode";

        /// <summary>
        /// Queue dead letter exchange argument
        /// </summary>
        public const string QueueArgsDeadLetterExchange = "x-dead-letter-exchange";

        /// <summary>
        /// Queue dead letter routing key argument
        /// </summary>
        public const string QueueArgsDeadLetterRoutingKey = "x-dead-letter-routing-key";

        /// <summary>
        /// Queue max priority argument
        /// </summary>
        public const string QueueArgsMaxPriority = "x-max-priority";

        /// <summary>
        /// RPC pseudo queue name for direct reply
        /// </summary>
        public const string RpcReplyToQueue = "amq.rabbitmq.reply-to";

        /// <summary>
        /// Default rpc default routing key
        /// </summary>
        public const string RpcDefaultRoutingKey = "ez.rpc";

        /// <summary>
        /// Header used to recreate to telemetry graph
        /// </summary>
        public const string TelemetryOperationIdHeaderName = "x-operation-id";
        
        /// <summary>
        /// Header used to recreate the telemetry graph
        /// </summary>
        public const string TelemetryParentOperationIdHeaderName = "x-operation-parent-id";

    }
}