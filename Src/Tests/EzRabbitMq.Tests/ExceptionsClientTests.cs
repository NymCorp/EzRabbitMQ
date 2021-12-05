using System;
using System.Threading.Tasks;
using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ExceptionsClientTests
    {
        private const string RoutingKey1 = "test-routingKey1";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private static readonly IProducerOptions ProducerRoutingKey1 = new DirectProducerOptions(RoutingKey1);

        private readonly ITestOutputHelper _output;

        public ExceptionsClientTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task CanHandleException()
        {
            var (mailbox, producer, _) = TestUtils.Build<DirectClientTests>(_output);

            using var consumer = mailbox.Create<Mailbox<TestSample>>(new DirectMailboxOptions(RoutingKey1, Guid.NewGuid().ToString()),
                ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            bool called = false;
            consumer.OnMessage += (_, _) =>
            {
                called = true;
                throw new Exception("test exception");
            };

            producer.Send(ProducerRoutingKey1, new TestSample(message));

            await Task.Delay(2000);

            Assert.True(called);
        }

        [Fact]
        public void ThrowUnderstandableExceptionOnBreakingChangeDetected()
        {
            var (mailbox, _, _) = TestUtils.Build<DirectClientTests>(_output);

            const string queueName = nameof(ThrowUnderstandableExceptionOnBreakingChangeDetected);
            using var _ = mailbox.Fanout<EmptyMessage>(queueName, consumerOptions: new ConsumerOptions
            {
                QueueDurable = false,
                QueueAutoDelete = true
            });

            Assert.Throws<BreakingChangeDetectedException>(() =>
            {
                using var m = mailbox.Fanout<TestSample>(queueName, consumerOptions: new ConsumerOptions
                {
                    QueueDurable = true,
                    QueueAutoDelete = true
                });
            });
        }
        
        // test Reflection.CachedReflection.GetType detect exception
    }
}