using System.IO;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class ConsoleWriter : StringWriter
    {
        private readonly ITestOutputHelper _output;

        public ConsoleWriter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override void WriteLine(string m)
        {
            _output.WriteLine(m);
        }
    }
}