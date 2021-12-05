
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace EzRabbitMQ
{
    /// <inheritdoc />
    public class MailboxService : IMailboxService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ValidationService _validationService;

        /// <summary>
        /// Service provides options validation and mailbox instances
        /// </summary>
        /// <param name="serviceProvider">Service provider used by <see cref="ActivatorUtilities"/>.</param>
        /// <param name="validationService">Service validation used to validate options.</param>
        public MailboxService(IServiceProvider serviceProvider, ValidationService validationService)
        {
            _serviceProvider = serviceProvider;
            _validationService = validationService;
        }

        /// <inheritdoc />
        public T Create<T>(IMailboxOptions options, ConsumerOptions? consumerOptions = null) where T : MailboxBase
        {
            consumerOptions ??= new ConsumerOptions();
            
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider, options, consumerOptions);
        }
        
        /// <inheritdoc />
        public Mailbox<T> Direct<T>(string routingKey, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null)
        {
            consumerOptions ??= new ConsumerOptions();

            queueName ??= Guid.NewGuid().ToString();
            
            var options = new DirectMailboxOptions(routingKey, queueName, exchangeName);

            _validationService.ValidateAndThrow(options);
                
            return ActivatorUtilities.CreateInstance<Mailbox<T>>(_serviceProvider, options, consumerOptions);
        }

        /// <inheritdoc />
        public Mailbox<T> Topic<T>(string routingKey, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null)
        {
            consumerOptions ??= new ConsumerOptions();

            queueName ??= Guid.NewGuid().ToString();
            
            var options = new TopicMailboxOptions(routingKey, queueName, exchangeName);
            
            _validationService.ValidateAndThrow(options);
            
            return ActivatorUtilities.CreateInstance<Mailbox<T>>(_serviceProvider, options, consumerOptions);
        }

        /// <inheritdoc />
        public Mailbox<T> Fanout<T>(string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null)
        {
            consumerOptions ??= new ConsumerOptions();

            queueName ??= Guid.NewGuid().ToString();
            
            var options = new FanoutMailboxOptions(queueName, exchangeName);
            
            _validationService.ValidateAndThrow(options);
            
            return ActivatorUtilities.CreateInstance<Mailbox<T>>(_serviceProvider, options, consumerOptions);
        }

        /// <inheritdoc />
        public Mailbox<T> Headers<T>(Dictionary<string, string> headers, XMatch xMatch, string? queueName = null, string? exchangeName = null, ConsumerOptions? consumerOptions = null)
        {
            consumerOptions ??= new ConsumerOptions();

            queueName ??= Guid.NewGuid().ToString();
            
            var options = new HeadersMailboxOptions(headers, xMatch, queueName, exchangeName);
            
            _validationService.ValidateAndThrow(options);
            
            return ActivatorUtilities.CreateInstance<Mailbox<T>>(_serviceProvider, options, consumerOptions);
        }

        /// <inheritdoc />
        public RpcClient RpcClient(string? serverName = null, ConsumerOptions? consumerOptions = null)
        {
            consumerOptions ??= new ConsumerOptions
            {
                QueueDurable = false,
                QueueAutoDelete = true,
                AutoAck = true
            };
            
            var options = new RpcClientMailboxOptions(serverName);
            
            _validationService.ValidateAndThrow(options);
            
            return ActivatorUtilities.CreateInstance<RpcClient>(_serviceProvider, options, consumerOptions);
        }

        /// <inheritdoc />
        public T RpcServer<T>(string? queueName = null, ConsumerOptions? consumerOptions = null) where T: RpcServerBase
        {
            consumerOptions ??= new ConsumerOptions
            {
                QueueAutoDelete = true, QueueDurable = false,
                AutoAck = true, QueueExclusive = true
            };
            
            var options = new RpcServerMailboxOptions(queueName);
            
            _validationService.ValidateAndThrow(options);

            return ActivatorUtilities.CreateInstance<T>(_serviceProvider, options, consumerOptions);
        }
    }
}