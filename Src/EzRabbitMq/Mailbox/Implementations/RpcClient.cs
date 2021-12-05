﻿
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ.Exceptions;
using EzRabbitMQ.Extensions;
using EzRabbitMQ.Reflection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace EzRabbitMQ
{
    /// <summary>
    /// Rpc client able to create request and receive response 
    /// </summary>
    public class RpcClient : MailboxBase
    {
        private readonly ConcurrentDictionary<Type, BlockingCollection<object>> _listeners = new();

        /// <summary>
        /// RpcClient can send request and wait for response
        /// </summary>
        public RpcClient(
            ILogger<RpcClient> logger,
            RpcClientMailboxOptions options,
            ISessionService session,
            ConsumerOptions consumerOptions
        )
            : base(logger, options, session, consumerOptions)
        {
        }

        /// <summary>
        /// Send request to the server and wait for a response.
        /// </summary>
        /// <param name="request">Request for the rpc server</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Task of nullable <see cref="IRpcResponse"/></returns>
        public Task<T?> CallAsync<T>(IRpcRequest request, CancellationToken cancellationToken) where T : class, IRpcResponse
        {
            return Task.Run(() => Call<T>(request, cancellationToken), cancellationToken);
        }


        /// <summary>
        /// Send request to the server and wait for a response.
        /// </summary>
        /// <param name="request">IRpcRequest you want to send to the server</param>
        /// <param name="externalCancellationToken">Optional CancellationToken preventing from waiting until request timeout if cancellation is raised</param>
        /// <typeparam name="T">IRpcResponse type</typeparam>
        /// <returns>Nullable IRpcResponse</returns>
        /// <exception cref="RpcClientCastException">The response received doesnt match with the method argument T type</exception>
        public T? Call<T>(IRpcRequest request, CancellationToken externalCancellationToken = default) where T : class, IRpcResponse
        {
           using var op = Session.Telemetry.Dependency(Options,
                "RpcClient request");

           if (Session.Model is null) return default;

            var blockingColl = _listeners.GetOrAdd(typeof(T), new BlockingCollection<object>());

            var sw = Stopwatch.StartNew();

            var body = Session.Config.SerializeData(request);

            var props = Session.Model.CreateBasicProperties();
            
            props.ReplyTo = Constants.RpcReplyToQueue;
            props.CorrelationId = Options.CorrelationId;
            props.Type = request.GetType().AssemblyQualifiedName;

            Session.Model.BasicPublish(ExchangeType.RpcServer.Name(), Options.RoutingKey, false, props, body);
            Logger.LogDebug("Message sent to {Exchange} routing Key: {RoutingKey}", ExchangeType.RpcServer.Name(), Options.RoutingKey);
            
            var timeoutCts = new CancellationTokenSource(ConsumerOptions.RpcCallTimeout);
            using var sharedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, externalCancellationToken);

            try
            {
                var response = blockingColl.Take(sharedCts.Token);

                sw.Stop();

                Logger.LogDebug("Call {CorrelationId} took {Elapsed}ms", Options.CorrelationId, sw.ElapsedMilliseconds.ToString());

                return (response as T ?? default) ?? throw new RpcClientCastException(typeof(T));
            }
            catch (OperationCanceledException)
            {
                Logger.LogWarning("RpcClient request has been cancelled after: {Timeout}ms for request: {Request}",
                    sw.ElapsedMilliseconds, JsonSerializer.Serialize(request));

                return default;
            }
        }

        /// <inheritdoc />
        protected override Task MessageHandle(object? sender, BasicDeliverEventArgs @event)
        {
            if (@event.BasicProperties.CorrelationId != Options.CorrelationId)
            {
                Logger.LogDebug("Received RPC call for another consumer: {CorrelationId}", Options.CorrelationId);
                
                return Task.CompletedTask;
            }

            var response = @event.GetData(Session.Config);

            var messageType = CachedReflection.GetType(@event.BasicProperties.Type);

            if (_listeners.TryGetValue(messageType, out var listener))
            {
                listener.Add(response);
            }
            
            Logger.LogDebug("Rpc response received for correlationId : {CorrelationId}", Options.CorrelationId);
            
            return Task.CompletedTask;
        }
    }
}