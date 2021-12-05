using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.ApplicationInsights.DataContracts;

namespace EzRabbitMQ
{
    /// <inheritdoc />
    public class MailboxExtractInformation : ExtractTelemetryInformationBase<IMailboxOptions>
    {
        private const string NoExchange = "no-exchange";
        private const string NoRoutingKey = "no-routing-key";

        private static readonly Lazy<string> EntryAssembly =
            new(() => Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown");

        /// <inheritdoc />
        public override RequestTelemetry CreateRequest(IMailboxOptions data, string operationName)
        {
            var request = new RequestTelemetry { Name = operationName };
            PopulateProperties(data, request.Properties);
            return request;
        }

        /// <inheritdoc />
        public override DependencyTelemetry CreateDependency(IMailboxOptions data, string operationName)
        {
            var dependency = new DependencyTelemetry
            {
                Name = data.QueueName,
                Target = $"{data.ExchangeName ?? "RPC"}({data.ExchangeType})-{data.QueueName}-{data.RoutingKey}",
                Data = $"[{EntryAssembly.Value}]-{data.ExchangeName ?? NoExchange}({data.ExchangeType})-{data.QueueName}-{data.RoutingKey ?? NoRoutingKey}",
                ResultCode = "200",
                Type = operationName
            };

            PopulateProperties(data, dependency.Properties);

            return dependency;
        }

        /// <inheritdoc />
        protected override void PopulateProperties(IMailboxOptions data, IDictionary<string, string> properties)
        {
            properties["assembly"] = EntryAssembly.Value;
            properties["exchange"] = data.ExchangeName ?? NoExchange;
            properties["exchange-type"] = data.ExchangeType.ToString();
            properties["queue-name"] = data.QueueName;
            properties["routing-key"] = data.RoutingKey ?? NoRoutingKey;
        }
    }
}