using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class HeadersClientTests: TestBase
    {
        private const string HeaderKeyA = "format";
        private const string HeaderKeyB = "type";

        private static readonly ConsumerOptions ConsumerOptions = TestUtils.ConsumerOptions;

        private static readonly Dictionary<string, string> ProducerRoutingKeyAJpgBChart =
            new()
            {
                { HeaderKeyA, "jpg" },
                { HeaderKeyB, "chart" }
            };

        private readonly ITestOutputHelper _output;

        public HeadersClientTests(ITestOutputHelper output) => _output = output;

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendHeadersMessageAndReceive(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<TopicClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Headers<TestSample>(new()
            {
                { HeaderKeyA, "jpg" },
                { HeaderKeyB, "chart" }
            }, XMatch.All, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.HeadersSend(ProducerRoutingKeyAJpgBChart, new TestSample(message)), consumer);

            logger.LogInformation("evt passed");
            Assert.Equal(message, evt.Data.Text);
            logger.LogInformation("assert passed");
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendHeadersAnyMessageAndReceive(bool isAsync)
        {
            var (mailbox, producer, logger) = TestUtils.Build<TopicClientTests>(_output, isAsync: isAsync);

            using var consumer = mailbox.Headers<TestSample>(new()
            {
                { HeaderKeyA, "jpg" },
                { HeaderKeyB, "chart" }
            }, XMatch.Any, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var evt = await TestUtils.Raises(logger,
                _ => producer.Send(new HeadersProducerOptions(
                    new Dictionary<string, string>
                    {
                        { HeaderKeyB, "chart" }
                    }), new TestSample(message)), consumer);

            Assert.Equal(message, evt.Data.Text);
        }

        [Theory]
        [MemberData(nameof(MailboxConfig))]
        public async Task CanSendHeadersAnyMessageAndNotReceive(bool isAsync)
        {
            var (mailbox, producer, _) = TestUtils.Build<TopicClientTests>(_output, isAsync: isAsync);

            var headersA = new Dictionary<string, string>
            {
                { HeaderKeyA, "jpg" },
                { HeaderKeyB, "chart" }
            };

            using var consumer = mailbox.Headers<TestSample>(headersA, XMatch.Any, consumerOptions: ConsumerOptions);

            var message = Guid.NewGuid().ToString();

            var called = false;
            consumer.OnMessage += (_, _) => called = true;

            var headersB = new Dictionary<string, string> { { "something", "else" } };
            producer.Send(new HeadersProducerOptions(headersB), new TestSample(message));

            await Task.Delay(2000);

            Assert.False(called);
        }
    }
}