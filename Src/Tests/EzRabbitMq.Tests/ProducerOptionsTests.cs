using System;
using System.Threading.Tasks;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ProducerOptionsTests : TestBase
    {
        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public ProducerOptionsTests(ITestOutputHelper output) => _output = output;

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
        public async Task CanSetExpirationOnMessage(bool isAsync)
        {
            var (mailboxService, producer, _) = TestUtils.Build<FanoutClientTests>(_output, isAsync: isAsync);

            var message = Guid.NewGuid().ToString();

            const string name = "expiration-test";

            var consumerOptions = new ConsumerOptions
            {
                ExchangeDurable = true,
                QueueDurable = true,
                QueueAutoDelete = false
            };
            
            var expiration = TimeSpan.FromSeconds(2);

            using (var _ = mailboxService.Fanout<EmptyMessage>(name, name, consumerOptions))
            {
            }

            var options = new FanoutProducerOptions(name)
            {
                Properties = {Expiration = expiration}
            };

            producer.Send(options, new TestSample(message));

            await Task.Delay(expiration.Add(TimeSpan.FromSeconds(1)));

            using var mailbox = mailboxService.Fanout<TestSample>(name, name, consumerOptions);

            bool called = false;
            mailbox.OnMessage += (_, _) => called = true;

            await Task.Delay(TimeSpan.FromSeconds(1));

            using var m = mailboxService.Fanout<TestSample>(name, name, new ConsumerOptions
            {
                ExchangeDurable = false,
                QueueDurable = false,
                QueueAutoDelete = true,
                ExchangeAutoDelete = true,
                QueueRecreateMode = RecreateMode.RecreateIfBreakingChangeDetected | RecreateMode.RecreateIfEmpty,
                ExchangeRecreateMode = RecreateMode.RecreateIfBreakingChangeDetected
            });

            Assert.False(called, "Should not receive message");
        }
    }
}