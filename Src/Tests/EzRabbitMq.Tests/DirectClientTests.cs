using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using FluentValidation;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class DirectClientTests
    {
        private const string RoutingKey1 = "routingKey1";
        private const string RoutingKey2 = "routingKey2";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public DirectClientTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void CanSendWithoutServer()
        {
            var (_, producer, _) = TestUtils.Build<DirectClientTests>(_output);
            
            var message = Guid.NewGuid().ToString();

            producer.DirectSend(RoutingKey1, new TestSample(message));
        }

        [Fact]
        public async Task SendAndReceiveUsingManualCreator()
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

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

        [Fact]
        public async Task RejectedMessageGoesToDlq()
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

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

        [Fact]
        public async Task SendAndReceive()
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

            using var consumer = mailbox.Direct<TestSample>(RoutingKey1, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger, _ =>
                producer.DirectSend(RoutingKey1, new TestSample(message)), consumer);

            Assert.Equal(message, evt1.Data.Text);
        }

        [Fact]
        public void CannotSendMessageWithNullRoutingKey()
        {
            var (mailbox, _, _) = TestUtils.Build<DirectClientTests>(_output);

            Assert.Throws<ValidationException>(() =>
                _ = mailbox.Direct<TestSample>(null!, consumerOptions: ConsumerOptions));
        }

        [Fact]
        public async Task CanSendDirectToMultipleReceivers()
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

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

        [Fact]
        public async Task MustNotSendMessageToAnotherQueue()
        {
            var (mailbox, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

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