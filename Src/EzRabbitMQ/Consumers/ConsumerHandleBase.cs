using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Extensions;
using EzRabbitMQ.Reflection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EzRabbitMQ;

/// <summary>
/// Application local state used to store global consumer indexes
/// </summary>
public static class AppState
{
    /// <summary>
    /// Store a counter for each assembly to scope consumer with readable name
    /// </summary>
    public static readonly Lazy<ConcurrentDictionary<string, int>> MailBoxIndexes = new(() => new ConcurrentDictionary<string, int>());
}

/// <summary>
/// Consumer handle basic session events
/// </summary>
public abstract class ConsumerHandleBase : IDisposable
{
    private const string OnMessageHandleName = nameof(IMailboxHandler<object>.OnMessageHandle);
    private const string OnMessageHandleAsyncName = nameof(IMailboxHandlerAsync<object>.OnMessageHandleAsync);

    /// <summary>
    /// Consumer tag, representing the unique consumer identifier
    /// </summary>
    protected readonly string ConsumerTag = ConsumersManager.CreateTag();

    /// <summary>
    /// Logger
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Mailbox options
    /// </summary>
    protected readonly IMailboxOptions Options;

    /// <summary>
    /// Current rabbitMQ session
    /// </summary>
    protected readonly ISessionService Session;

    /// <summary>
    /// Consumer options
    /// </summary>
    protected readonly ConsumerOptions ConsumerOptions;

    /// <summary>
    /// The service handle RabbitMQ events
    /// </summary>
    /// <param name="logger">ILogger</param>
    /// <param name="options">Mailbox options, some option will be used to create exchange/queue/bindings</param>
    /// <param name="session">Current RabbitMQ session</param>
    /// <param name="consumerOptions">Consumer options, these options are used to change the consumer behavior, see <see cref="ConsumerOptions"/></param>
    protected ConsumerHandleBase(
        ILogger logger,
        IMailboxOptions options,
        ISessionService session,
        ConsumerOptions consumerOptions
    )
    {
        Logger = logger;
        Session = session;
        Options = options;
        ConsumerOptions = consumerOptions;

        Session.Polly.TryExecute<CreateConsumerException>(CreateConsumer);
    }

    /// <summary>
    /// RabbitMQ Consumer
    /// </summary>
    protected IBasicConsumer? Consumer { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Session.Dispose();
    }

    private void CreateConsumer()
    {
        using var operation = Session.Telemetry.Request(Options, "BasicConsumer created");
        Consumer = Session.Config.IsAsyncDispatcher ? CreateAsyncConsumer() : CreateSyncConsumer();
    }

    private IBasicConsumer CreateSyncConsumer()
    {
        var consumer = new EventingBasicConsumer(Session.Model);
        consumer.Shutdown += Consumer_Shutdown;
        consumer.Received += Consumer_Received;
        consumer.Registered += Consumer_Registered;
        consumer.Unregistered += Consumer_Unregistered;
        consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
        return consumer;
    }

    private IBasicConsumer CreateAsyncConsumer()
    {
        var consumer = new AsyncEventingBasicConsumer(Session.Model);
        consumer.Shutdown += AsyncConsumer_Shutdown;
        consumer.Received += AsyncConsumer_Received;
        consumer.Registered += AsyncConsumer_Registered;
        consumer.Unregistered += AsyncConsumer_Unregistered;
        consumer.ConsumerCancelled += AsyncConsumer_ConsumerCancelled;
        return consumer;
    }

    private Task AsyncConsumer_Registered(object? sender, ConsumerEventArgs @event)
    {
        Session.Telemetry.Trace("Consumer registered", new() {{"consumerTag", ConsumerTag}});

        return Task.CompletedTask;
    }

    private void Consumer_Registered(object? sender, ConsumerEventArgs @event)
    {
        AsyncConsumer_Registered(sender, @event).GetAwaiter().GetResult();
    }

    private Task AsyncConsumer_Received(object? sender, BasicDeliverEventArgs @event)
    {
        using var operation = Session.Telemetry.Dependency(Options, "OnMessageHandle");

        operation.Telemetry.Context.Operation.SetTelemetry(@event.BasicProperties);

        return MessageHandle(sender, @event);
    }

    private void Consumer_Received(object? sender, BasicDeliverEventArgs @event)
    {
        AsyncConsumer_Received(sender, @event).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Event raised on message received by the consumer.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="event">RabbitMQ raw event.</param>
    protected virtual async Task MessageHandle(object? sender, BasicDeliverEventArgs @event)
    {
        var messageTypeText = @event.BasicProperties.Type;

        var currentType = GetType();
        var method = CachedReflection.FindMethodToInvoke(currentType, messageTypeText, OnMessageHandleName);
        var asyncMethod = CachedReflection.FindMethodToInvoke(currentType, messageTypeText, OnMessageHandleAsyncName);
        if (method is null && asyncMethod is null)
        {
            Logger.LogError("Not found handle exception");
            throw new ReflectionNotFoundHandleException(GetType().Name, $"{OnMessageHandleName}|{OnMessageHandleAsyncName}", messageTypeText);
        }

        var message = @event.GetMessage(Session.Config);

        await TryExecuteOnMessage(@event, message, method, asyncMethod);
    }

    private async Task TryExecuteOnMessage(BasicDeliverEventArgs @event, object message, MethodBase? method, MethodInfo? asyncMethod)
    {
        try
        {
            if (asyncMethod is not null)
            {
                var task = asyncMethod.Invoke(this, new[] {message, Session.SessionToken});
                if (task is not null)
                {
                    await (dynamic) task;
                }
            }
            else
            {
                if (method is null)
                {
                    Logger.LogError("Unable to find any matching method called: {MethodName} in your implementation : {ImplementationName}", OnMessageHandleName, GetType().Name);
                    HandleOnMessageException(@event);
                    return;
                }

                method.Invoke(this, new[] {message});
            }

            if (!ConsumerOptions.AutoAck)
            {
                Session.Model?.BasicAck(@event.DeliveryTag, ConsumerOptions.AckMultiple);
            }
        }
        catch
        {
            HandleOnMessageException(@event);
            throw;
        }
    }

    private void HandleOnMessageException(BasicDeliverEventArgs @event)
    {
        if (ConsumerOptions.AutoAck)
        {
            return;
        }

        Session.Model?.BasicReject(@event.DeliveryTag, false);
    }

    private Task AsyncConsumer_ConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        Session.Telemetry.Trace("Consumer cancelled", new() {{"consumerTag", ConsumerTag}});

        return Task.CompletedTask;
    }

    private void Consumer_ConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        AsyncConsumer_ConsumerCancelled(sender, e).GetAwaiter().GetResult();
    }

    private Task AsyncConsumer_Shutdown(object? sender, ShutdownEventArgs e)
    {
        if (e.ReplyCode != 200)
        {
            Logger.LogError("Consumer shutdown : {ReplyText}", e.ReplyText);

            CreateConsumer();

            return Task.CompletedTask;
        }

        Logger.LogDebug("Consumer shutdown {ConsumerTag}", ConsumerTag);

        Session.Telemetry.Trace("Consumer shutdown", new() {{"consumerTag", ConsumerTag}});

        return Task.CompletedTask;
    }

    private void Consumer_Shutdown(object? sender, ShutdownEventArgs e)
    {
        AsyncConsumer_Shutdown(sender, e).Wait();
    }

    private Task AsyncConsumer_Unregistered(object? sender, ConsumerEventArgs e)
    {
        Session.Telemetry.Trace("Consumer unregistered", new() {{"consumerTag", ConsumerTag}});

        Logger.LogDebug("Consumer unregistered : {ConsumerTag}", ConsumerTag);

        return Task.CompletedTask;
    }

    private void Consumer_Unregistered(object? sender, ConsumerEventArgs e)
    {
        AsyncConsumer_Unregistered(sender, e).GetAwaiter().GetResult();
    }
}