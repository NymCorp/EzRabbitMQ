namespace EzRabbitMQ;

/// <summary>
/// Producer properties
/// </summary>
public record ProducerProperties
{
    /// <summary>
    /// Set the message priority
    /// </summary>
    public byte? Priority { get; set; } = default;

    /// <summary>
    /// Set the Reply To property
    /// </summary>
    public string? ReplyTo { get; set; }

    /// <summary>
    /// CorrelationId used for RPC
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Message expiration
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Message's headers
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    /// Delivery Mode (NonPersistent/{Persistent)
    /// </summary>
    public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.Persistent;
}