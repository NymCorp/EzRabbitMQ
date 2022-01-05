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
        private readonly IAdditionService _additionService;

        public RpcServerTest(
            ILogger<RpcServerTest> logger,
            IMailboxOptions options,
            ISessionService session,
            ConsumerOptions consumerOptions,
            IRandomService randomService,
            IAdditionService additionService
        ) : base(logger, options, session, consumerOptions)
        {
            _randomService = randomService;
            _additionService = additionService;
        }

        public RpcSampleResponse Handle(RpcSampleRequest request)
        {
            Logger.LogDebug("Handled called, generating random number: {RandomNumber}", _randomService.GetRandomInt());
            return new RpcSampleResponse
            {
                Model = new TestSample(request.Text)
            };
        }

        public Task<RpcIncrementResponse> HandleAsync(RpcIncrementRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new RpcIncrementResponse(_additionService.Add(request.CurrentValue, 1)));
        }
    }
}