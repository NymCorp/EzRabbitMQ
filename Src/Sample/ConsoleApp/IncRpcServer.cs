using EzRabbitMQ;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;

public class IncRpcServer : RpcServerBase, IRpcServerHandle<RpcIncrementResponse, RpcIncrementRequest>
{
    public IncRpcServer(ILogger<IncRpcServer> logger, IMailboxOptions options, ISessionService session, ConsumerOptions consumerOptions) 
        : base(logger, options, session, consumerOptions)
    {
    }

    public RpcIncrementResponse Handle(RpcIncrementRequest request)
    {
        return new RpcIncrementResponse
        {
            NewValue = request.CurrentValue + 1
        };
    }
}