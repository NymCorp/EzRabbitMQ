using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ.Tests
{
    public class RpcServerTest : RpcServerBase, 
        IRpcServerHandle<RpcSampleResponse, RpcSampleRequest>,
        IRpcServerHandle<RpcIncrementResponse, RpcIncrementRequest>
    {
        public RpcServerTest(
            ILogger<RpcServerTest> logger,
            IMailboxOptions options,
            ISessionService session,
            ConsumerOptions consumerOptions
        ) : base(logger, options, session, consumerOptions)
        {
        }

        public RpcSampleResponse Handle(RpcSampleRequest request)
        {
            return new RpcSampleResponse
            {
                Model = new TestSample(request.Text)
            };
        }

        public RpcIncrementResponse Handle(RpcIncrementRequest request)
        {
            return new()
            {
                NewValue = request.CurrentValue + 1
            };
        }
    }
}