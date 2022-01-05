namespace EzRabbitMQ;

/// <summary>
/// Enum type of handled RabbitMQ exception type. 
/// </summary>
// ReSharper disable once InconsistentNaming
public enum RabbitMQExceptionType
{
    /// <summary>
    /// Not recognized exception type.
    /// </summary>
    Unknown,

    /// <summary>
    /// RabbitMQ inequivalent argument exception detected.
    /// </summary>
    InequivalentArg,

    /// <summary>
    /// RabbitMQ queue not found exception detected.
    /// </summary>
    QueueNotFound,

    /// <summary>
    /// RabbitMQ exchange not found exception detected.
    /// </summary>
    ExchangeNotFound,

    /// <summary>
    /// A queue with the same name already exists with exclusive mode
    /// </summary>
    QueueIsExclusiveAndAlreadyExists
}