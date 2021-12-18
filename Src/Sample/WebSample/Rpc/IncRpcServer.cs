using EzRabbitMQ;
using EzRabbitMQ.Tests.Messages;

public class IncRpcServer : RpcServerBase, IRpcServerHandleAsync<RpcIncrementResponse, RpcIncrementRequest>
{
    private readonly IAdditionService _math;

    public IncRpcServer(
        ILogger<IncRpcServer> logger,
        IMailboxOptions options,
        ISessionService session,
        ConsumerOptions consumerOptions,
        IAdditionService math)
        : base(logger, options, session, consumerOptions)
    {
        _math = math;
        logger.LogInformation("RPC SERVER READY");
    }

    public Task<RpcIncrementResponse> HandleAsync(RpcIncrementRequest request, CancellationToken cancellationToken)
    {
        var result = _math.Addition(request.CurrentValue, 1);
        Logger.LogInformation("Received call, returning {Result}", result);
        return Task.FromResult(new RpcIncrementResponse(result));
    }
}