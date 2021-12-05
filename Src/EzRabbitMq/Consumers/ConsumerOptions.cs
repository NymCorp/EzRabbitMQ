using System;
using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary> 
    /// Consumer options are here to configure the consumer behaviors like exchange options, prefetch options, queue options and some more
    /// </summary>
    public record ConsumerOptions : IExchangeOptions, IPrefetchOptions, IQueueOptions
    {
        /// <summary>
        /// Enable auto acknowledge <c>true</c> by default
        /// if set to false the system will ack is no exception occured in the handle
        /// <value>true</value>
        /// </summary>
        public bool AutoAck { get; init; } = true;

        /// <inheritdoc />
        public bool QueueDurable { get; init; } = true;

        /// <inheritdoc />
        public bool QueueAutoDelete { get; init; }

        /// <inheritdoc />
        public bool AckMultiple { get; init; } = false;

        /// <inheritdoc />
        public bool ExchangeDurable { get; init; } = true;

        /// <inheritdoc />
        public bool ExchangeAutoDelete { get; init; }

        /// <inheritdoc />
        public RecreateMode ExchangeRecreateMode { get; init; } = RecreateMode.None;

        /// <inheritdoc />
        public Dictionary<string, object> ExchangeDeclareArguments { get; init; } = new ();

        /// <inheritdoc />
        public uint PrefetchSize  => 0;

        /// <inheritdoc />
        public ushort PrefetchCount { get; init; }

        /// <inheritdoc />
        public bool PrefetchGlobal { get; init; } = false;

        /// <inheritdoc />
        public bool QueueExclusive { get; init; } = false;

        /// <inheritdoc />
        public int QueueSizeLimit { get; init; }

        /// <inheritdoc />
        public string? DeadLetterExchangeName { get; init; }

        /// <inheritdoc />
        public string? DeadLetterRoutingKey { get; init; }

        /// <inheritdoc />
        public byte? QueueMaxPriority { get; init; } = default;
        
        /// <summary>
        /// From the moment of the send of the request a cancellation token source will raise a cancel after the amount of time set in <see cref="RpcCallTimeout"/> 
        /// </summary>
        public TimeSpan RpcCallTimeout { get; init; } = TimeSpan.FromSeconds(5);

        /// <inheritdoc />
        public QueueMode QueueMode { get; init; } = QueueMode.Default;

        /// <inheritdoc />
        public RecreateMode QueueRecreateMode { get; init; } = RecreateMode.None;

        /// <inheritdoc />
        public Dictionary<string, object> QueueDeclareArguments { get; init; } = new ();

        /// <inheritdoc />
        public Dictionary<string, object> QueueBindArguments { get; init; } = new ();
    }
}