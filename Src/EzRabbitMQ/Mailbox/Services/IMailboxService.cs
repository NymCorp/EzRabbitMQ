namespace EzRabbitMQ;

/// <summary>
/// Instantiate mailboxes.
/// </summary>
public interface IMailboxService
{
    /// <summary>
    /// Instantiate a mailbox of type T.
    /// </summary>
    /// <param name="options">Mailbox options see <see cref="IMailboxOptions"/>.</param>
    /// <param name="consumerOptions">Consumer options see <see cref="ConsumerOptions"/>.</param>
    /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Mailbox implementation.</typeparam>
    /// <returns>Disposable instance of mailbox.</returns>
    T Create<T>(IMailboxOptions options, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null) where T : MailboxBase;

    /// <summary>
    /// Create and return a Direct type mailbox.
    /// </summary>
    /// <param name="routingKey">Keyword or tag, it's like the address target of the message.</param>
    /// <param name="queueName">Queue name, if not defined a uniq name is created.</param>
    /// <param name="exchangeName">Exchange name, a default value is provided based on the exchange type (direct).</param>
    /// <param name="consumerOptions">Consumer options.</param>
    /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Data type you want to receive.</typeparam>
    /// <returns>Mailbox.</returns>
    Mailbox<T> Direct<T>(string routingKey, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null);


    /// <summary>
    /// Create and return a Topic type mailbox.
    /// </summary>
    /// <param name="routingKey">Topic routing key, must be word split by dots with # or *.</param>
    /// <param name="queueName">Queue name, if not defined a uniq name is created.</param>
    /// <param name="exchangeName">Exchange name, a default value is provided based on the exchange type (topic).</param>
    /// <param name="consumerOptions">Consumer options.</param>
    /// /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Data type you want to receive.</typeparam>
    /// <returns>Mailbox.</returns>
    Mailbox<T> Topic<T>(string routingKey, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null);

    /// <summary>
    /// Create and return a Fanout mailbox.
    /// </summary>
    /// <param name="queueName">Queue name, if not defined a uniq name is created.</param>
    /// <param name="exchangeName">Exchange name, a default value is provided based on the exchange type (fanout).</param>
    /// <param name="consumerOptions">Consumer options.</param>
    /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Data type you want to receive.</typeparam>
    /// <returns>Mailbox.</returns>
    Mailbox<T> Fanout<T>(string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null);

    /// <summary>
    /// Create and return a Headers mailbox.
    /// </summary>
    /// <param name="headers">>Headers : e.g.: {"format": "excel"}.</param>
    /// <param name="xMatch">Match type, all or any.</param>
    /// <param name="queueName">Queue name, if not defined a uniq name is created.</param>
    /// <param name="exchangeName">Exchange name, a default value is provided based on the exchange type (headers).</param>
    /// <param name="consumerOptions">Consumer options.</param>
    /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Data type you want to receive.</typeparam>
    /// <returns>Headers.</returns>
    Mailbox<T> Headers<T>(Dictionary<string, string> headers, XMatch xMatch, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null);

    /// <summary>
    /// Create and return a RpcClient.
    /// </summary>
    /// <param name="serverQueueName">Server's queueName, fallbacks to default server queue name.</param>
    /// <param name="consumerOptions">Override default consumer options.</param>
    /// <returns>RpcClient.</returns>
    RpcClient RpcClient(string? serverQueueName = null, ConsumerOptions? consumerOptions = null);

    /// <summary>
    /// Create and return a RpcServer of type T implementing RpcServerBase.
    /// </summary>
    /// <param name="queueName">Override default queue name.</param>
    /// <param name="consumerOptions">Override default consumer Options.</param>
    /// <param name="customServiceProvider">Override the default service provider used, allowing you to control when dispose instances.</param>
    /// <typeparam name="T">Implementation of RpcServerBase.</typeparam>
    /// <returns>Disposable instance of T.</returns>
    T RpcServer<T>(string? queueName = null, ConsumerOptions? consumerOptions = null, IServiceProvider? customServiceProvider = null) where T : RpcServerBase;
}