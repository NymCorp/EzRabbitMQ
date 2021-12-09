
using System;
using System.Text.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace EzRabbitMQ
{
    /// <summary>
    /// Config bag for EzRabbitMQ library.
    /// </summary>
    public class EzRabbitMQConfig
    {
        /// <summary>
        /// Polly policy used for rabbitmq client/server requests.
        /// </summary>
        public AsyncRetryPolicy PollyPolicy { get; private set; } =
            Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(5, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

        /// <summary>
        /// Is async dispatcher or sync dispatcher, default value to false, async dispatcher doesnt guarantee message order.
        /// </summary>
        public bool IsAsyncDispatcher { get; set; }

        /// <summary>
        /// IConnectionFactory with default value targeting localhost.
        ///  <example>Connection.UserName = "example-user";</example>
        /// </summary>
        public ConnectionFactory Connection { get; private set; } = new ()
        {
            HostName = "localhost",
            Password = "guest",
            UserName = "guest",
            Port = 5672,
            VirtualHost = "/",
        };

        /// <summary>
        /// Data serializer.
        /// </summary>
        public Func<object, byte[]> SerializeData { get; private set; } = obj => JsonSerializer.SerializeToUtf8Bytes(obj);

        /// <summary>
        /// Data deserializer.
        /// </summary>
        public Func<byte[], Type, object?> DeserializeData { get; private set; } = (bytes, type) => JsonSerializer.Deserialize(bytes, type);

        /// <summary>
        /// Optional AppInsight InstrumentationKey used for telemetry and tracing.
        /// </summary>
        public string InstrumentationKey { get; private set; } = string.Empty;

        /// <summary>
        /// Set the instrumentationKey to send in AppInsight metrics and trace.
        /// </summary>
        /// <param name="instrumentationKey">AppInsight instrumentation key.</param>
        public void ConfigureInstrumentationKey(string instrumentationKey) => InstrumentationKey = instrumentationKey;

        /// <summary>
        /// Configure the rabbitMQ connection factory.
        /// </summary>
        /// <param name="connection"><see cref="ConnectionFactory"/> instance.</param>
        public void ConfigureConnection(ConnectionFactory connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// Override the current polly policy, this policy is used to wrap all rabbitMQ calls.
        /// </summary>
        /// <param name="policy">New polly policy.</param>
        public void ConfigurePollyPolicy(AsyncRetryPolicy policy)
        {
            PollyPolicy = policy;
        }

        /// <summary>
        /// Configure the serializer and deserializer used to serialize messages.
        /// </summary>
        /// <param name="serializer">Takes an object and return a byte[].</param>
        /// <param name="deserializer">Takes a byte array and return an object.</param>
        public void ConfigureSerialization(Func<object, byte[]> serializer, Func<byte[], Type, object?> deserializer)
        {
            SerializeData = serializer;
            DeserializeData = deserializer;
        }
    }
}