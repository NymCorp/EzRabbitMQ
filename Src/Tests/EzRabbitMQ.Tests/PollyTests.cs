using System;
using EzRabbitMQ.Resiliency;
using Polly;
using Xunit;

namespace EzRabbitMQ.Tests
{
    internal class ExampleException : Exception
    {
        public ExampleException(Exception e) : base("example exception", e)
        {
        }
    }

    public class PollyTests
    {
        [Fact]
        public void CanConfigureHandleException()
        {
            Assert.Throws<ExampleException>(() =>
            {
                var config = new EzRabbitMQConfig();
                config.ConfigurePollyPolicy(Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(1, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    ));

                var pollyServer = new PollyService(config);
                pollyServer.TryExecute<ExampleException>(() => throw new Exception());
            });
        }
    }
}