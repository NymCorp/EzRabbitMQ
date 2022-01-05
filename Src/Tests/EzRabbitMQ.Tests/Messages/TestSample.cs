using System.Collections.Generic;

namespace EzRabbitMQ.Tests.Messages
{
    public record TestSample(string Text);

    public record TestSample2
    {
        public  Dictionary<string, string> Tags { get; init; }
    }
}