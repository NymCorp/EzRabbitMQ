using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;

namespace EzRabbitMQ.Tests
{
    public class RpcServerTest : RpcServerBase, 
        IRpcServerHandle<RpcSampleResponse, RpcSampleRequest>,
        IRpcServerHandleAsync<RpcIncrementResponse, RpcIncrementRequest>
    {
        // here to validate IServiceProvider resolving from user's service instance 
        private readonly IRandomService _randomService;

        public RpcServerTest(
            ILogger<RpcServerTest> logger,
            IMailboxOptions options,
            ISessionService session,
            ConsumerOptions consumerOptions,
            IRandomService randomService
        ) : base(logger, options, session, consumerOptions)
        {
            _randomService = randomService;
        }

        public RpcSampleResponse Handle(RpcSampleRequest request)
        {
            return new RpcSampleResponse
            {
                Model = new TestSample(request.Text)
            };
        }

        public Task<RpcIncrementResponse> HandleAsync(RpcIncrementRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new RpcIncrementResponse()
            {
                NewValue = request.CurrentValue + 1
            });
        }
    }
}