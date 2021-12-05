using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class FanoutClientTests
    {
        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public FanoutClientTests(ITestOutputHelper output) => _output = output;

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

        [Fact]
        public async Task CanSendDirectToMultipleReceivers()
        {
            var (mailbox, producer, logger) = TestUtils.Build<FanoutClientTests>(_output);

            using var consumer = mailbox.Fanout<TestSample>(consumerOptions: ConsumerOptions);
            using var consumer2 = mailbox.Fanout<TestSample>(consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.FanoutSend(new TestSample(message)), consumer);

            var evt2 = await TestUtils.Raises(logger,
                _ => producer.FanoutSend(new TestSample(message)), consumer2);

            Assert.Equal(message, evt2.Data.Text);
            Assert.Equal(message, evt.Data.Text);
        }
    }
}