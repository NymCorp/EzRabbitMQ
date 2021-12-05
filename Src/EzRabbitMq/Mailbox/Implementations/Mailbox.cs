using System;
using EzRabbitMQ.Messages;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ
{
    /// <summary>
    /// Event mailbox raises event when a message is received
    /// </summary>
    /// <typeparam name="T">Messages type</typeparam>
    public class Mailbox<T> : MailboxBase, IMailboxHandler<T>
    {
        /// <inheritdoc />
        public Mailbox(
            ILogger<Mailbox<T>> logger,
            IMailboxOptions options,
            ISessionService session,
            ConsumerOptions consumerOptions
        )
            : base(logger, options, session, consumerOptions)
        {
        }

        /// <summary>
        /// Event raised when a message is received
        /// </summary>
        public event EventHandler<IMessage<T>>? OnMessage;

        /// <inheritdoc />
        public virtual void OnMessageHandle(IMessage<T> message)
        {
            OnMessage?.Invoke(this, message);
        }
    }
}