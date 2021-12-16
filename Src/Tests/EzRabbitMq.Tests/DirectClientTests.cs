using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using FluentValidation;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class DirectClientTests : TestBase
    {
        private const string RoutingKey1 = "routingKey1";
        private const string RoutingKey2 = "routingKey2";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public DirectClientTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public void CanSendWithoutServer(bool isAsync)
        {
            var (_, producer, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            var message = Guid.NewGuid().ToString();

            producer.DirectSend(RoutingKey1, new TestSample(message));
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task SendAndReceiveUsingManualCreator(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            var options = new DirectMailboxOptions(RoutingKey1, Guid.NewGuid().ToString());

            using var consumer = mailbox.Create<Mailbox<TestSample>>(options, ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger,
                _ =>
                {
                    using var p = producer.Create(new DirectProducerOptions(RoutingKey1));
                    p.Publish(new TestSample(message));
                }, consumer);

            Assert.Equal(message, evt1.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task RejectedMessageGoesToDlq(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            const string dlqExchange = "dlq-exchange";
            const string dlqRoutingKey = "dlq";

            using var consumer = mailbox.Direct<TestSample>(RoutingKey1,
                consumerOptions: ConsumerOptions with
                {
                    AutoAck = false,
                    DeadLetterExchangeName = dlqExchange,
                    DeadLetterRoutingKey = dlqRoutingKey
                });

            using var dlqConsumer = mailbox.Fanout<TestSample>("dlq", "dlq-exchange",
                ConsumerOptions with {ExchangeDurable = false, ExchangeAutoDelete = true});

            var message = Guid.NewGuid().ToString();

            consumer.OnMessage += (_, _) => throw new Exception("Fake exception");

            var evt1 = await TestUtils.Raises(logger, _ =>
                producer.DirectSend(RoutingKey1, new TestSample(message)), dlqConsumer);

            Assert.NotNull(evt1);

            Assert.Equal(message, evt1.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task SendAndReceive(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Direct<TestSample>(RoutingKey1, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger, _ =>
                producer.DirectSend(RoutingKey1, new TestSample(message)), consumer);

            Assert.Equal(message, evt1.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public void CannotSendMessageWithNullRoutingKey(bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            Assert.Throws<ValidationException>(() =>
                _ = mailbox.Direct<TestSample>(null!, consumerOptions: ConsumerOptions));
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendDirectToMultipleReceivers(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Direct<TestSample>(RoutingKey1, consumerOptions: ConsumerOptions);
            using var consumer2 = mailbox.Direct<TestSample>(RoutingKey2, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();
            var data = new TestSample(message);

            var evt1 = await TestUtils.Raises(logger,
                _ => producer.DirectSend(RoutingKey1, data), consumer);

            Assert.Equal(message, evt1.Data.Text);

            var evt2 = await TestUtils.Raises(logger,
                _ => producer.DirectSend(RoutingKey2, data), consumer2);

            Assert.Equal(message, evt2.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task MustNotSendMessageToAnotherQueue(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Direct<TestSample>(RoutingKey1, consumerOptions: ConsumerOptions);

            using var consumer2 = mailbox.Direct<TestSample>(RoutingKey2, consumerOptions: ConsumerOptions);

            var called = false;
            consumer2.OnMessage += (_, _) => called = true;

            var message = Guid.NewGuid().ToString();

            await TestUtils.Raises(logger, _ => producer.DirectSend(RoutingKey1, new TestSample(message)),
                consumer);

            Assert.False(called);
        }
    }
}