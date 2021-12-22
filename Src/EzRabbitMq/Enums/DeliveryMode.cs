namespace EzRabbitMQ;

/// <summary>
/// Message's delivery mode
/// </summary>
public enum DeliveryMode
{
    /// <summary>
    /// Non persistent
    /// </summary>
    NonPersistent = 1,

    /// <summary>
    /// Persistent
    /// </summary>
    Persistent = 2
}