using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using EzRabbitMQ.Tests.Models.CustomMailboxes;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    
    public class CustomClientTests: TestBase
    {
        private const string RoutingKey1 = "routingKey1";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public CustomClientTests(ITestOutputHelper output) => _output = output;
        
        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task SendAndReceiveUsingManualCreator(bool isAsync)
        {
            var (mailbox, producer, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

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