namespace EzRabbitMQ;

/// <summary>
/// Adding an Handle method on your RpcServer to receive a TResponse
/// </summary>
/// <typeparam name="TResponse">The response type returned for the request TRequest</typeparam>
/// <typeparam name="TRequest">The request type to receive</typeparam>
public interface IRpcServerHandle<TResponse, in TRequest>
{
    /// <summary>
    /// Handle called when a request is received
    /// </summary>
    /// <param name="request">The request type to receive</param>
    /// <returns>The response type returned for the request TRequest</returns>
    TResponse Handle(TRequest request);
}

/// <summary>
/// Adding an async Handle method on your RpcServer to receive TResponse
/// </summary>
/// <typeparam name="TResponse">The response type returned for the request TRequest</typeparam>
/// <typeparam name="TRequest">The request type to receive</typeparam>
public interface IRpcServerHandleAsync<TResponse, in TRequest>
{
    /// <summary>
    /// Handle called when a request is received
    /// </summary>
    /// <param name="request">The request type to receive</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>The response type returned for the request TRequest</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Simple implementation of the rpc server handle
/// </summary>
public interface IRpcServerHandle : IRpcServerHandle<EmptyRpcResponse, EmptyRpcRequest>
{
}