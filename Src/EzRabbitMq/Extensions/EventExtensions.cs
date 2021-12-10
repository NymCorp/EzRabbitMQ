using System;
using System.Linq;
using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Reflection;
using RabbitMQ.Client.Events;

namespace EzRabbitMQ.Extensions
{
    /// <summary>
    /// Extensions methods used for data unwrapping <see cref="IMessage{T}"/> or raw data from rabbitMQ message
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Get data type from message's property "Type" and read message body bytes and deserialize data and return the object
        /// </summary>
        /// <param name="event">RabbitMQ event message</param>
        /// <param name="config">Current library config</param>
        /// <exception cref="InvalidOperationException">Throw exception is deserialization fail</exception>
        /// <returns>Deserialized data</returns>
        public static object GetData(this BasicDeliverEventArgs @event, EzRabbitMQConfig config)
        {
            var messageType = CachedReflection.GetType(@event.BasicProperties.Type);

            if (messageType is null)
            {
                throw new InvalidOperationException(
                    $"Unable to deserialize message data to type: {messageType}");
            }

            return config.DeserializeData(@event.Body.ToArray(), messageType)
                   ?? throw new InvalidOperationException(
                       $"Unable to deserialize message data to type: {messageType}");;
        }

        /// <summary>
        /// Get Data and wrap it into a IMessage  
        /// </summary>
        /// <param name="event">RabbitMQ event message</param>
        /// <param name="config">Current library config</param>
        /// <exception cref="ReflectionNotFoundTypeException">Throw exception if message type is not found</exception>
        /// <returns>Return a <see cref="IMessage{T}"/></returns>
        public static object GetMessage(this BasicDeliverEventArgs @event, EzRabbitMQConfig config)
        {
            var messageType = CachedReflection.GetType(@event.BasicProperties.Type);

            if (messageType is null)
            {
                throw new ReflectionNotFoundTypeException(@event.BasicProperties.Type);
            }

            var obj = @event.GetData(config);

            var realType = typeof(Message<>).MakeGenericType(messageType);

            var constructor = realType.GetConstructor(new[] {typeof(BasicDeliverEventArgs), messageType});

            return constructor!.Invoke(new[] {@event, obj});
        }
    }
}