using System;
using System.Threading.Tasks;
using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ExceptionsClientTests: TestBase
    {
        private const string RoutingKey1 = "test-routingKey1";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private static readonly IProducerOptions ProducerRoutingKey1 = new DirectProducerOptions(RoutingKey1);

        private readonly ITestOutputHelper _output;

        public ExceptionsClientTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanHandleException(bool isAsync)
        {
            var (mailbox, producer, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Create<Mailbox<TestSample>>(new DirectMailboxOptions(RoutingKey1, Guid.NewGuid().ToString()),
                ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            int cpt = 0;
            consumer.OnMessage += (_, _) =>
            {
                cpt++;
                throw new Exception("test exception");
            };
            const int nbMessages = 10;
            for(var i = 0; i < nbMessages; i++)
            {
                producer.Send(ProducerRoutingKey1, new TestSample(message));
            }

            await Task.Delay(2000);

            Assert.Equal(nbMessages, cpt);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public void ThrowUnderstandableExceptionOnBreakingChangeDetected(bool isAsync)
        {
            var (mailbox, _, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

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
        
        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task ThrowReflectionExceptionWhenReceivingBadMessage(bool isAsync)
        {
            var (mailbox, producer, _) = TestUtils.Build<DirectClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Create<Mailbox<TestSample>>(new DirectMailboxOptions(RoutingKey1, Guid.NewGuid().ToString()),
                ConsumerOptions);

            var called = false;
            consumer.OnMessage += (_, _) => called = true;

            producer.Send(ProducerRoutingKey1, new TestSample2());

            await Task.Delay(2000);

            Assert.False(called);
        }
    }
}