using RabbitMQ.Client;

namespace EzRabbitMQ;

/// <summary>
/// Handle the rabbitMQ connection
/// </summary>
public interface IConnectionService : IDisposable
{
    /// <summary>
    /// RabbitMQ connection
    /// </summary>
    IConnection? Connection { get; }

    /// <summary>
    /// Create a new connection using poly policy
    /// </summary>
    void TryConnect();
}