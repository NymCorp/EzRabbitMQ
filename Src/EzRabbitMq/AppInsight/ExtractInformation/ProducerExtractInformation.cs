using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.ApplicationInsights.DataContracts;

namespace EzRabbitMQ
{
    /// <inheritdoc />
    public class ProducerExtractInformation : ExtractTelemetryInformationBase<IProducerOptions>
    {
        private static readonly Lazy<string> EntryAssembly = new(() => Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown");

        /// <inheritdoc />
        public override RequestTelemetry CreateRequest(IProducerOptions data, string operationName)
        {
            var request = new RequestTelemetry
            {
                Name = operationName
            };

            PopulateProperties(data, request.Properties);
            return request;
        }

        /// <inheritdoc />
        public override DependencyTelemetry CreateDependency(IProducerOptions data, string operationName)
        {
            var dependency = new DependencyTelemetry
            {
                Name = operationName,
                Target = $"{data.ExchangeName}-{data.RoutingKey}",
                Data = $"[{EntryAssembly.Value}]-{data.ExchangeName}-{data.RoutingKey}",
                Type = operationName,
                ResultCode = "200"
            };
            
            PopulateProperties(data, dependency.Properties);

            return dependency;
        }

        /// <inheritdoc />
        protected override void PopulateProperties(IProducerOptions data, IDictionary<string, string> properties)
        {
            properties["assembly"] = EntryAssembly.Value;
            properties["exchange"] = data.ExchangeName;
            properties["routing-key"] = data.RoutingKey;
        }
    }
}