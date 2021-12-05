using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using FluentValidation;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class TopicClientTests
    {
        private const string RoutingKeyA = "root.a.*";
        private const string RoutingKeyB = "root.b.*";

        private const string ProducerKeyA = "root.a.toto";
        private const string ProducerKeyA2 = "root.a.titi";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private readonly ITestOutputHelper _output;

        public TopicClientTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task CanSendTopicMessageAndReceive()
        {
            var (mailbox, producer, logger) = TestUtils.Build<TopicClientTests>(_output);

            using var consumer = mailbox.Topic<TestSample>(RoutingKeyA, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.TopicSend(ProducerKeyA, new TestSample(message)), consumer);

            Assert.Equal(message, evt.Data.Text);
        }

        [Fact]
        public async Task CanSendTopicToMultipleReceivers()
        {
            var (mailbox, producer, logger) = TestUtils.Build<TopicClientTests>(_output);

            using var consumer = mailbox.Topic<TestSample>(RoutingKeyA, consumerOptions: ConsumerOptions);
            using var consumer2 = mailbox.Topic<TestSample>(RoutingKeyA, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.TopicSend(ProducerKeyA, new TestSample(message)), consumer);

            Assert.Equal(message, evt.Data.Text);

            var evt2 = await TestUtils.Raises(logger,
                _ => producer.TopicSend(ProducerKeyA2, new TestSample(message)), consumer2);

            Assert.Equal(message, evt2.Data.Text);
        }

        [Fact]
        public async Task MustNotSendMessageToAnotherQueue()
        {
            var (mailbox, producer, logger) = TestUtils.Build<TopicClientTests>(_output);

            using var consumer = mailbox.Topic<TestSample>(RoutingKeyA, consumerOptions: ConsumerOptions);
            using var consumer2 = mailbox.Topic<TestSample>(RoutingKeyB, consumerOptions: ConsumerOptions);

            var called = false;
            consumer2.OnMessage += (_, _) => called = true;

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.TopicSend(ProducerKeyA, new TestSample(message)), consumer);

            Assert.False(called);
            Assert.Equal(message, evt.Data.Text);
        }
        
        [Fact]
        public void CanValidateInvalidRoutingKey()
        {
            var (_,producer, _) = TestUtils.Build<TopicClientTests>(_output);
        
            Assert.Throws<ValidationException>(() =>
            {
                producer.TopicSend("toto", new TestSample("titi"));
            });
        }
    }
}