using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Divergic.Logging.Xunit;
using EzRabbitMQ.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public static class TestUtils
    {
        public static (IMailboxService, IProducerService, ILogger<T>) Build<T>(ITestOutputHelper output)
        {
            
            var sp = new ServiceCollection()
                .AddSingleton(LogFactory.Create(output))
                .AddLogging()
                .AddEzRabbitMQ(config =>
                {
                    output.WriteLine($"virtual host detected : {config.Connection.VirtualHost}");
                    var cs = Environment.GetEnvironmentVariable("EzRabbitMQ__ConnectionString");
                    if (!string.IsNullOrWhiteSpace(cs))
                    {
                        config.Connection.Uri = new Uri(cs);
                    }
                })
                .BuildServiceProvider();

            var mailbox = sp.GetService<IMailboxService>();
            var producer = sp.GetService<IProducerService>();
            var logger = sp.GetService<ILogger<T>>();

            return (mailbox, producer, logger);
        }

        public static Task<IMessage<T>> Raises<T>(ILogger logger, Action<IMessage<T>> triggered, Mailbox<T> mailbox,
            int timeout = 10000)
        {
            object parameter = default(IMessage<T>);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            var bc = new BlockingCollection<IMessage<T>>();

            void InternalListener(object sender, IMessage<T> data)
            {
                bc.Add(data, cts.Token);
                parameter = data;
                mailbox.OnMessage -= InternalListener;

                logger.LogInformation("unittest event listener unregistered");
            }

            mailbox.OnMessage += InternalListener;
            logger.LogInformation("unittest event listener registered");

            triggered(parameter as IMessage<T>);
            logger.LogInformation("unittest triggered called");

            return Task.FromResult(bc.Take(cts.Token));
        }

        public static readonly ConsumerOptions ConsumerOptions = new()
        {
            QueueAutoDelete = true, QueueDurable = false, RpcCallTimeout = TimeSpan.FromSeconds(1)
        };
    }
}