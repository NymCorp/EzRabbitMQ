using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Benchmark.Models;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using EzRabbitMQ;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net50)]
    [MarkdownExporter, HtmlExporter]
    [MemoryDiagnoser]
    [ThreadingDiagnoser] 
    public class MessagePackSerializer
    {
        private static readonly ConsumerOptions ConsumerOptions = new()
        {
            QueueAutoDelete = true,
            QueueDurable = false,
            QueueSizeLimit = 1000
        };

        private readonly Dictionary<string, (IProducerService ProducerService, IMailboxService MailboxService)> _serviceProviders;

        public MessagePackSerializer()
        {
            _serviceProviders = new()
            {
                {"message-pack", CreateServiceProvider(true)},
                {"default", CreateServiceProvider(false)}
            };
        }

        private static (IProducerService ProducerService, IMailboxService MailboxService) CreateServiceProvider(bool isMessagePack)
        {
            var services =  new ServiceCollection()
                .AddLogging()
                .AddEzRabbitMQ(config =>
                {
                    if (isMessagePack)
                    {
                        ConfigureMessagePack(config);
                    }

                    Configure(config);
                })
                .BuildServiceProvider();

            var mailboxService = services.GetRequiredService<IMailboxService>();
            var producerService = services.GetRequiredService<IProducerService>();
            
            return (producerService, mailboxService);
        }

        private static void Configure(EzRabbitMQConfig config)
        {
            var cs = Environment.GetEnvironmentVariable("EzRabbitMQ__ConnectionString");
            if (!string.IsNullOrWhiteSpace(cs))
            {
                config.Connection.Uri = new Uri(cs);
            }
        }

        private static void ConfigureMessagePack(EzRabbitMQConfig config)
        {
            StaticCompositeResolver.Instance.Register(
                StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePack.MessagePackSerializer.DefaultOptions = option;

            config.ConfigureSerialization(data => MessagePack.MessagePackSerializer.Serialize(data),
                (bytes, type) => MessagePack.MessagePackSerializer.Deserialize(type, bytes));
        }


        [Benchmark, Arguments("default")]
        [Arguments("message-pack")]
        public bool SendMessage(string providerName)
        {
            _serviceProviders[providerName].ProducerService
                .DirectSend("bench", new RpcIncrementRequest(0));
            return true;
        }

        [Benchmark, Arguments("default")]
        [Arguments("message-pack")]
        public string CreateMailbox(string providerName)
        {
            var id = Guid.NewGuid().ToString();
            
            using var mailbox = _serviceProviders[providerName].MailboxService.Direct<RpcIncrementRequest>(id, consumerOptions: ConsumerOptions);

            return id;
        }

        [Benchmark, Arguments("default")]
        [Arguments("message-pack")]
        public string SendAndReceiveMessage(string providerName)
        {
            var mailboxService = _serviceProviders[providerName].MailboxService;
            var producerService = _serviceProviders[providerName].ProducerService;
        
            var id = Guid.NewGuid().ToString();
            using var mailbox = mailboxService.Direct<RpcIncrementRequest>(id, consumerOptions: ConsumerOptions);
        
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            
            using var bc = new BlockingCollection<string>();
        
            mailbox.OnMessage += (_, message) =>
            {
                if (!bc.IsCompleted)
                {
                    bc.Add(message.Data.CurrentValue.ToString(), ct.Token);
                }
            };
            
            producerService.DirectSend(id, new RpcIncrementRequest(0));
        
            return bc.Take(ct.Token);
        }

        [Benchmark, Arguments("default")]
        [Arguments("message-pack")]
        public RpcIncrementResponse? MessagePackSendAndReceiveRpcMessage(string providerName)
        {
            var id = Guid.NewGuid().ToString();

            var server = $"server-{id}";

            var mailboxService = _serviceProviders[providerName].MailboxService;

            using var mailbox = mailboxService.RpcServer<IncrementRpcServer>(server, ConsumerOptions);

            using var client = mailboxService.RpcClient(server, ConsumerOptions);

            var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest(0));

            return response;
        }

        [Benchmark, Arguments("default")]
        [Arguments("message-pack")]
        public int SendAndReceive100RpcMessage(string providerName)
        {
            var id = Guid.NewGuid().ToString();

            var server = $"server-{id}";

            var mailboxService = _serviceProviders[providerName].MailboxService;

            using var mailbox = mailboxService.RpcServer<IncrementRpcServer>(server, ConsumerOptions);

            using var client = mailboxService.RpcClient(server, ConsumerOptions);

            for (int i = 0; i < 100; i++)
            {
                client.Call<RpcIncrementResponse>(new RpcIncrementRequest(0));
            }

            return 0;
        }
    }

    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}