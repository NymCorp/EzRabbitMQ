using EzRabbitMQ;
using Microsoft.Extensions.Logging;

namespace Benchmark.Models
{
    public class IncrementRpcServer: RpcServerBase, IRpcServerHandle<RpcIncrementResponse, RpcIncrementRequest>
    {
        public IncrementRpcServer(
            ILogger<IncrementRpcServer> logger, 
            IMailboxOptions options, 
            ISessionService session, 
            ConsumerOptions consumerOptions
            ) 
            : base(logger, options, session, consumerOptions)
        {
        }

        public RpcIncrementResponse Handle(RpcIncrementRequest request)
        {
            Logger.LogInformation("rpc received");
            return new RpcIncrementResponse(request.CurrentValue +1);
        }
    }
}