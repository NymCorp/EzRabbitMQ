namespace EzRabbitMQ;

/// <summary>
/// Producer options
/// </summary>
public interface IProducerOptions
{
    /// <summary>
    /// The routing key is the target destination of your message
    /// For direct exchange type e.g.: "sample"
    /// For topic exchange type e.g.: "sample.*"
    /// For fanout or headers this can stay unset
    /// </summary>
    string RoutingKey { get; }

    /// <summary>
    /// The exchange you want to sent your message to, default values are set :
    /// direct => amq.direct
    /// topic => amp.topic
    /// You can override the exchange name of you need.
    /// </summary>
    string ExchangeName { get; }

    /// <summary>
    /// Producer properties
    /// </summary>
    ProducerProperties Properties { get; }
}