
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Extensions;
using EzRabbitMQ.Reflection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EzRabbitMQ
{
    /// <summary>
    /// Application local state used to store global consumer indexes
    /// </summary>
    public static class AppState
    {
        /// <summary>
        /// Store a counter for each assembly to scope consumer with readable name
        /// </summary>
        public static readonly ConcurrentDictionary<string, int> MailBoxIndexes = new();
    }

    /// <summary>
    /// Consumer handle basic session events
    /// </summary>
    public abstract class ConsumerHandleBase : IDisposable
    {
        private const string OnMessageHandleName = nameof(IMailboxHandler<object>.OnMessageHandle);

        /// <summary>
        /// Consumer tag
        /// </summary>
        protected readonly string ConsumerTag;

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

            ConsumerTag = ConsumersManager.CreateTag();

            ConsumerOptions = consumerOptions;

            Session.Polly.TryExecute<CreateConsumerException>(CreateConsumer);
        }

        /// <summary>
        /// RabbitMQ Consumer
        /// </summary>
        protected AsyncEventingBasicConsumer? Consumer { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            Session.Dispose();
        }

        private void CreateConsumer()
        {
            using var operation = Session.Telemetry.Request(Options, "BasicConsumer created");

            var consumer = new AsyncEventingBasicConsumer(Session.Model);
            consumer.Shutdown += Consumer_Shutdown;
            consumer.Received += Consumer_Received;
            consumer.Registered += Consumer_Registered;
            consumer.Unregistered += Consumer_Unregistered;
            consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
            Consumer = consumer;
        }

        private Task Consumer_Registered(object? sender, ConsumerEventArgs @event)
        {
            Session.Telemetry.Trace("Consumer registered", new() {{"consumerTag", ConsumerTag}});

            return Task.CompletedTask;
        }

        /// <summary>
        /// Event raised on message received by the consumer
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="event">RabbitMQ raw event</param>
        protected Task Consumer_Received(object? sender, BasicDeliverEventArgs @event)
        { 
            using var operation = Session.Telemetry.Dependency(Options, "OnMessageHandle");
            
            operation.Telemetry.Context.Operation.SetTelemetry(@event.BasicProperties);

            return MessageHandle(sender, @event);
        }
        
        /// <summary>
        /// Event raised on message received by the consumer
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="event">RabbitMQ raw event</param>
        protected virtual Task MessageHandle(object? sender, BasicDeliverEventArgs @event)
        {   
            var messageTypeText = @event.BasicProperties.Type;

            try
            {
                var method = CachedReflection.FindMethodToInvoke(GetType(), messageTypeText, OnMessageHandleName);

                var message = @event.GetMessage(Session.Config);

                TryExecuteOnMessage(@event, message, method);
            }
            catch (ReflectionNotFoundTypeException e)
            {
                Logger.LogError(e, "Not found type exception");
                Session.Model?.BasicReject(@event.DeliveryTag, false);
            }
            catch (ReflectionNotFoundHandleException e)
            {
                Logger.LogError(e, "Not found handle exception");
                Session.Model?.BasicReject(@event.DeliveryTag, false);
            }

            return Task.CompletedTask;
        }

        private void TryExecuteOnMessage(BasicDeliverEventArgs @event, object message, MethodBase invoker)
        {
            try
            {
                invoker.Invoke(this, new[] {message});

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

        private Task Consumer_ConsumerCancelled(object? sender, ConsumerEventArgs e)
        {
            Session.Telemetry.Trace("Consumer cancelled", new() {{"consumerTag", ConsumerTag}});
            
            return Task.CompletedTask;
        }

        private Task Consumer_Shutdown(object? sender, ShutdownEventArgs e)
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

        private Task Consumer_Unregistered(object? sender, ConsumerEventArgs e)
        {
            Session.Telemetry.Trace("Consumer unregistered", new() {{"consumerTag", ConsumerTag}});

            Logger.LogDebug("Consumer unregistered : {ConsumerTag}", ConsumerTag);
            
            return Task.CompletedTask;
        }
    }
}