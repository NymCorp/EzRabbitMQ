using System;
using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
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

        public void OnMessageHandle(IMessage<TestSample2> message)
        {
            SyncCalled = true;
        }
    }
    
    public class CustomClientTests
    {
        private const string RoutingKey1 = "routingKey1";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public CustomClientTests(ITestOutputHelper output) => _output = output;
        
        [Fact]
        public async Task SendAndReceiveUsingManualCreator()
        {
            var (mailbox, producer, _) = TestUtils.Build<DirectClientTests>(_output);

            var options = new DirectMailboxOptions(RoutingKey1, Guid.NewGuid().ToString());

            using var consumer = mailbox.Create<CustomMailbox>(options, ConsumerOptions);

            var message = Guid.NewGuid().ToString();
            
            using var p = producer.Create(new DirectProducerOptions(RoutingKey1));
            p.Publish(new TestSample(message));
            
            p.Publish(new TestSample2
            {
                Tags = new ()
                {
                    {"type", "file"},
                    {"size", "heavy"}
                } 
            });

            await Task.Delay(TimeSpan.FromSeconds(2));
            
            Assert.True(consumer.AsyncCalled);
            Assert.True(consumer.SyncCalled);
        }
    }
}