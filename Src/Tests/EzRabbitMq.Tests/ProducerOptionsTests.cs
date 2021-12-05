using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ProducerOptionsTests
    {
        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public ProducerOptionsTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task CanSendFanoutMessageAndReceive()
        {
            var (mailbox, producer, logger) = TestUtils.Build<FanoutClientTests>(_output);

            using var consumer = mailbox.Fanout<TestSample>(consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger,
                _ => producer.FanoutSend(new TestSample(message)), consumer);
            Assert.Equal(message, evt1.Data.Text);
        }
        
        // test expiration
        // test priority
        
        // test data is null
    }
}