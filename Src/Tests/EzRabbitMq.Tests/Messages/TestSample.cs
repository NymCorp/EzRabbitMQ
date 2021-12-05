using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EzRabbitMQ.Tests.Messages
{
    public record TestSample(string Text);

    public record TestSample2
    {
        public  ImmutableArray<string> Texts { get; }

        public TestSample2(IEnumerable<string> elements)
        {
            Texts = elements.OrderBy(e => e)
                .ToImmutableArray();
        }
    }
}