using System.Threading;
using System.Threading.Tasks;
using EzRabbitMQ;
using EzRabbitMQ.Tests.Messages;
using Microsoft.Extensions.Logging;

public class IncRpcServer : RpcServerBase, IRpcServerHandleAsync<RpcIncrementResponse, RpcIncrementRequest>
{
    public IncRpcServer(ILogger<IncRpcServer> logger, IMailboxOptions options, ISessionService session, ConsumerOptions consumerOptions) 
        : base(logger, options, session, consumerOptions)
    {
    }

    public Task<RpcIncrementResponse> HandleAsync(RpcIncrementRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new RpcIncrementResponse
        {
            NewValue = request.CurrentValue + 1
        });
    }
}