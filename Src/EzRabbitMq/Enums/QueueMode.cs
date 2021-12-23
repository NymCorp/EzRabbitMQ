namespace EzRabbitMQ;

/// <summary>
/// Queue possible modes
/// </summary>
public enum QueueMode
{
    /// <summary>
    /// Lazy mode
    /// </summary>
    Lazy,

    /// <summary>
    /// Default mode
    /// </summary>
    Default,

    /// <summary>
    /// Quorum mode
    /// </summary>
    Quorum
}

internal static class QueueModeExtensions
{
    public static string GetTextValue(this QueueMode mode) => mode switch
    {
        QueueMode.Default => "default",
        QueueMode.Lazy => "lazy",
        QueueMode.Quorum => "quorum",
        _ => throw new ArgumentOutOfRangeException(nameof(mode), $"Not expected x-queue-mode value: {mode}")
    };
}