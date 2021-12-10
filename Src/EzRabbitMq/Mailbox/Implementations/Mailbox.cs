using System;
using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ.Messages;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ
{
    /// <summary>
    /// Event mailbox raises event when a message is received
    /// </summary>
    /// <typeparam name="T">Messages type</typeparam>
    public class Mailbox<T> : MailboxBase, IMailboxHandlerAsync<T>
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
        public virtual Task OnMessageHandleAsync(IMessage<T> message, CancellationToken cancellationToken = default)
        {
            OnMessage?.Invoke(this, message);
            return Task.CompletedTask;
        }
    }
}