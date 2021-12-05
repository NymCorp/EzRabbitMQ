using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// IProducerService
    /// </summary>
    public interface IProducerService
    {
        /// <summary>
        /// Returns an new instance of Producer
        /// </summary>
        /// <param name="options">Producer options</param>
        /// <returns>Disposable instance of <see cref="Producer"/></returns>
        Producer Create(IProducerOptions options);

        /// <summary>
        /// Create and close a scope to send a unique message
        /// </summary>
        /// <param name="options"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        void Send<T>(IProducerOptions options, T data);

        /// <summary>
        /// Create a transient producer to send a direct type message 
        /// </summary>
        /// <param name="routingKey">Message's routing key</param>
        /// <param name="data">Data you want to send to the queue</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <typeparam name="T">Data type of the message you want to send</typeparam>
        void DirectSend<T>(string routingKey, T data, string? exchangeName = null);

        /// <summary>
        /// Create and return a disposable producer for exchange type direct
        /// </summary>
        /// <param name="routingKey">Message's routing key</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <returns>Disposable producer</returns>
        Producer Direct(string routingKey, string? exchangeName = null!);

        /// <summary>
        /// Create and return a disposable producer for exchange type topic
        /// </summary>
        /// <param name="routingKey">Message's routing key</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <returns>Disposable producer</returns>
        Producer Topic(string routingKey, string? exchangeName = null!);

        /// <summary>
        /// Create and return a disposable producer for exchange type topic
        /// </summary>
        /// <param name="routingKey">Message's routing key</param>
        /// <param name="data">Data you want to send to the queue</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <returns>Disposable producer</returns>
        void TopicSend<T>(string routingKey, T data, string? exchangeName = null!);

        /// <summary>
        /// Create and return a disposable producer for exchange type headers
        /// </summary>
        /// <param name="headers">Message's headers</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <returns>Disposable producer</returns>
        Producer Headers(Dictionary<string, string> headers, string? exchangeName = null!);

        /// <summary>
        /// Create a transient producer to send a headers type message 
        /// </summary>
        /// <param name="headers">Message's headers</param>
        /// <param name="data">Data you want to send to the queue</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <typeparam name="T">Data type of the message you want to send</typeparam>
        void HeadersSend<T>(Dictionary<string, string> headers, T data, string? exchangeName = null!);

        /// <summary>
        /// Create and return a disposable producer for exchange type fanout
        /// </summary>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <returns>Disposable producer</returns>
        Producer Fanout(string? exchangeName = null!);

        /// <summary>
        /// Create a transient producer to send a fanout type message 
        /// </summary>
        /// <param name="data">Data you want to send to the queue</param>
        /// <param name="exchangeName">Exchange name, fallback to default type's exchange name</param>
        /// <typeparam name="T">Data type of the message you want to send</typeparam>
        void FanoutSend<T>(T data, string? exchangeName = null);
    }
}