using EzRabbitMQ.Messages;

namespace EzRabbitMQ
{
    /// <summary>
    /// Implementation of IMailbox can receive matching T type messages
    /// </summary>
    /// <typeparam name="T">type of the data in the message</typeparam>
    public interface IMailboxHandler<T>
    {
        /// <summary>
        /// Method called when a T message is received
        /// </summary>
        /// <param name="message">message containing the T data</param>
        public void OnMessageHandle(IMessage<T> message);
    }
}