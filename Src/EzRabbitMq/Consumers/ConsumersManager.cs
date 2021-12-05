using System.Reflection;

namespace EzRabbitMQ
{
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

            if (!AppState.MailBoxIndexes.ContainsKey(entryName))
            {
                AppState.MailBoxIndexes.TryAdd(entryName, 0);
            }

            ++AppState.MailBoxIndexes[entryName];

            return $"{entryName}({AppState.MailBoxIndexes[entryName]})";
        }
        
    }
}