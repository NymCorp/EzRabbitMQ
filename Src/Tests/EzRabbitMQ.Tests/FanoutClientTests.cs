using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class FanoutClientTests: TestBase
    {
        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public FanoutClientTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendFanoutMessageAndReceive(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<FanoutClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Fanout<TestSample>(consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger,
                _ => producer.FanoutSend(new TestSample(message)), consumer);
            Assert.Equal(message, evt1.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendDirectToMultipleReceivers(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<FanoutClientTests>(_output, isAsync: isAsync);

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