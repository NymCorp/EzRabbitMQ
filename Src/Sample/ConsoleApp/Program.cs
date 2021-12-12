// See https://aka.ms/new-console-template for more information

using System;
using System.Reflection;
using EzRabbitMQ;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");

var host = Host
    .CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder => builder.AddUserSecrets(Assembly.GetEntryAssembly(), true))
    .ConfigureServices((context, services) =>
    {
        services
            .AddEzRabbitMQ(config =>
            {
                if (context.Configuration.TryGet(ConfigurationKeys.AppInsightKey, out var ik))
                {
                    config.ConfigureInstrumentationKey(ik);
                }
            });
    })
    .Build();

var producerService = host.Services.GetRequiredService<IProducerService>();
var mailboxService = host.Services.GetRequiredService<IMailboxService>();

using var mailboxA = mailboxService.Direct<Sample>("a", "mailbox-a");
mailboxA.OnMessage += (_, message) => Console.WriteLine($"[a] Received message: {message.Data}");

using var mailboxB = mailboxService.Direct<Sample>("b", "mailbox-b");
mailboxB.OnMessage += (_, message) =>
{
    var newMessage = message.Data with { Age = message.Data.Age + 1 };
    producerService.DirectSend("c",  newMessage);
    Console.WriteLine($"[b]Received message: {message.Data}");
};

using var mailboxC = mailboxService.Direct<Sample>("c", "mailbox-c");
mailboxC.OnMessage += (_, message) =>
{
    var newMessage = message.Data with { Age = message.Data.Age + 1 };
    producerService.DirectSend("a",  newMessage);
    Console.WriteLine($"[c]Received message: {message.Data}");
};

producerService.DirectSend("b",  new Sample("Toto", 42));

using var rpcClient = mailboxService.RpcClient();
using var rpcServer = mailboxService.RpcServer<IncRpcServer>();

var response = rpcClient.Call<RpcIncrementResponse>(new RpcIncrementRequest(42));

Console.WriteLine($"rpc response: {response?.NewValue.ToString() ?? "null"}");

Console.WriteLine("Press any key to exit...");
Console.ReadLine();
