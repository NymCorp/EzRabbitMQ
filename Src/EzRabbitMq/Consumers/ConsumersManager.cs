namespace EzRabbitMQ;

/// <summary>
/// Provides consumer tags.
/// </summary>
public class ConsumersManager
{
    /// <summary>
    /// Return a assembly unique id for the consumer.
    /// </summary>
    /// <returns>Assembly unique id</returns>
    public static string CreateTag()
    {
        var entryName = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown-entry";

        if (!AppState.MailBoxIndexes.Value.ContainsKey(entryName))
        {
            AppState.MailBoxIndexes.Value.TryAdd(entryName, 0);
        }

        ++AppState.MailBoxIndexes.Value[entryName];

        return $"{entryName}({AppState.MailBoxIndexes.Value[entryName]})";
    }
}