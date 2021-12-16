using EzRabbitMQ;
using Microsoft.Extensions.Logging;

namespace Benchmark.Models
{
    public class IncrementRpcServer : RpcServerBase, IRpcServerHandle<RpcIncrementResponse, RpcIncrementRequest>
    {
        public IncrementRpcServer(ILogger<IncrementRpcServer> logger, IMailboxOptions options, ISessionService session, ConsumerOptions consumerOptions)
            : base(logger, options, session, consumerOptions)
        {
        }

        public RpcIncrementResponse Handle(RpcIncrementRequest request) => new(request.CurrentValue + 1);
    }
    
    public class MsgPackIncrementRpcServer : RpcServerBase, IRpcServerHandle<MsgPackRpcIncrementResponse, MsgPackRpcIncrementRequest>
    {
        public MsgPackIncrementRpcServer(ILogger<MsgPackIncrementRpcServer> logger, IMailboxOptions options, ISessionService session, ConsumerOptions consumerOptions)
            : base(logger, options, session, consumerOptions)
        {
        }

        public MsgPackRpcIncrementResponse Handle(MsgPackRpcIncrementRequest request) => new(request.CurrentValue + 1);
    }
}