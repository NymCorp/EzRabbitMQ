using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ.Tests.Models.CustomMailboxes
{
    public class CustomMailbox : MailboxBase, IMailboxHandlerAsync<TestSample>, IMailboxHandler<TestSample2>
    {
        public bool AsyncCalled { get; private set; }
        public bool SyncCalled { get; private set; }
        
        public CustomMailbox(ILogger<CustomMailbox> logger, IMailboxOptions options, ISessionService session, ConsumerOptions consumerOptions) : base(logger, options, session, consumerOptions)
        {
        }

        public Task OnMessageHandleAsync(IMessage<TestSample> message, CancellationToken cancellationToken)
        {
            AsyncCalled = true;
            return Task.CompletedTask;
        }

        public void OnMessageHandle(IMessage<TestSample2> message) => SyncCalled = true;
    }
}