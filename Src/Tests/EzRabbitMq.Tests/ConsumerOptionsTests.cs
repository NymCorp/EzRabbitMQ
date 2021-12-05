using System;
using System.Threading.Tasks;
using EzRabbitMQ.Messages;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ConsumerOptionsTests
    {
        private readonly ITestOutputHelper _output;

        public ConsumerOptionsTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task CanRecreateIfBreakingChangeDetected()
        {
            var (mailboxService, producer, logger) = TestUtils.Build<DirectClientTests>(_output);

            const string queueName = "queue-recreation-test";
            
            {
                using var _ = mailboxService.Fanout<EmptyMessage>(queueName, consumerOptions: new ConsumerOptions
                {
                    QueueDurable = true
                });
            }

            using var m = mailboxService.Fanout<TestSample>(queueName, consumerOptions: new ConsumerOptions
            {
                QueueDurable = false, 
                QueueRecreateMode = RecreateMode.RecreateIfBreakingChangeDetected,
                QueueAutoDelete = true 
            });
            
            var message = Guid.NewGuid().ToString();

            var evt1 = await TestUtils.Raises(logger,
                _ =>
                {
                    producer.FanoutSend(new TestSample(message));
                }, m);

            Assert.Equal(message, evt1.Data.Text);
        }
        
        [Fact]
        public void CanRecreateExchangeOnForceRecreate()
        {
            var (mailboxService, _, _) = TestUtils.Build<DirectClientTests>(_output);

            const string queueName = "exchange-recreation-test";
            const string exchangeName = "test-recreate";
            {
                using var _ = mailboxService.Fanout<EmptyMessage>(queueName, exchangeName, new ConsumerOptions
                {
                    ExchangeDurable = true,
                    QueueDurable = false,
                    QueueAutoDelete = true
                });
            }

            using var m = mailboxService.Fanout<TestSample>(queueName,  exchangeName, new ConsumerOptions
            {
                ExchangeDurable = false,
                QueueDurable = false,
                QueueAutoDelete = true,
                ExchangeAutoDelete = true,
                ExchangeRecreateMode = RecreateMode.ForceRecreate
            });
        }

        [Fact]
        public void CanRecreateExchangeOnBreakingChange()
        {
            var (mailboxService, _, _) = TestUtils.Build<DirectClientTests>(_output);

            const string queueName = "exchange-recreation-test";
            const string exchangeName = "test-recreate";

            {
                using var _ = mailboxService.Fanout<EmptyMessage>(queueName, exchangeName, new ConsumerOptions
                {
                    ExchangeDurable = true,
                    QueueDurable = false,
                    QueueAutoDelete = true,
                });
            }
            
            using var m = mailboxService.Fanout<TestSample>(queueName, exchangeName, new ConsumerOptions
            {
                ExchangeDurable = false,
                QueueDurable = false,
                QueueAutoDelete = true,
                ExchangeAutoDelete = true,
                QueueRecreateMode = RecreateMode.RecreateIfBreakingChangeDetected | RecreateMode.RecreateIfEmpty,
                ExchangeRecreateMode = RecreateMode.RecreateIfBreakingChangeDetected
            });
        }
    }
}