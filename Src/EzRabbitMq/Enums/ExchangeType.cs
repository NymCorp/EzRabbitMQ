using System;

namespace EzRabbitMQ
{
    /// <summary>
    /// Possible exchange types
    /// </summary>
    public enum ExchangeType
    {
        /// <summary>
        /// Direct type
        /// </summary>
        Direct,
        /// <summary>
        /// Topic type
        /// </summary>
        Topic,
        /// <summary>
        /// Fanout type
        /// </summary>
        Fanout,
        /// <summary>
        /// Headers type
        /// </summary>
        Headers,
        /// <summary>
        /// RPC Exchange type Client
        /// </summary>
        RpcClient,
        /// <summary>
        /// RPC Exchange type server
        /// </summary>
        RpcServer
    }
    
    internal static class ExchangeTypeExtensions
    {
        public static string Name(this ExchangeType exchangeType)
        {
            return exchangeType switch
            {
                ExchangeType.Direct => "amq.direct",
                ExchangeType.Fanout => "amq.fanout",
                ExchangeType.Topic => "amq.topic",
                ExchangeType.Headers => "amq.headers",
                ExchangeType.RpcServer => "amq.direct",
                ExchangeType.RpcClient => string.Empty,
                _ => throw new InvalidOperationException("Missing string value from exchange type")
            };
        }

        public static string Type(this ExchangeType exchangeType)
        {
            return exchangeType switch
            {
                ExchangeType.Direct => "direct",
                ExchangeType.Fanout => "fanout",
                ExchangeType.Topic => "topic",
                ExchangeType.Headers => "headers",
                ExchangeType.RpcClient or ExchangeType.RpcServer => "direct",
                _ => throw new InvalidOperationException("Missing string type from exchange type")
            };
        }
    }
}