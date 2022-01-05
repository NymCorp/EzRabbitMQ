namespace EzRabbitMQ;

/// <summary>
/// Headers match kind
/// </summary>
public enum XMatch
{
    /// <summary>
    /// Must match all headers
    /// </summary>
    All,

    /// <summary>
    /// Must match one of the headers
    /// </summary>
    Any
}

internal static class XMatchExtensions
{
    public static string GetTextValue(this XMatch match) => match switch
    {
        XMatch.Any => "any",
        XMatch.All => "all",
        _ => throw new InvalidOperationException("XMatch value cannot be found")
    };
}