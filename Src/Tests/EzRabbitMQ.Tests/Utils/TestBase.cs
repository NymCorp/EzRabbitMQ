using System.Collections.Generic;

namespace EzRabbitMQ.Tests
{
    public class TestBase
    {
        public static IEnumerable<object[]> RpcConfig =>
            new List<object[]>
            {
                new object[] {true, true},
                new object[] {true, false},
                new object[] {false, true},
                new object[] {false, false}
            };

        public static IEnumerable<object[]> MailboxConfig()
        {
            yield return new object[] {true};
            yield return new object[] {false};
        }
    }
}