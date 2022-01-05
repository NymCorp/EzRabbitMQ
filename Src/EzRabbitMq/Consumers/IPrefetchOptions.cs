namespace EzRabbitMQ;

/// <summary>
/// Perfect options
/// </summary>
public interface IPrefetchOptions
{
    /// <summary>
    /// PrefetchSize parameter, not settable because it creates exception :
    /// <code>NOT_IMPLEMENTED - prefetch_size!=0</code>
    /// Default: true
    /// </summary>
    uint PrefetchSize { get; }

    /// <summary>
    /// Set the number of message the consumer will prefetch
    /// Default: no limit
    /// </summary>
    ushort PrefetchCount { get; }

    /// <summary>
    /// Set prefetch count is global or per consumer
    /// Default: false
    /// </summary>
    bool PrefetchGlobal { get; }
}