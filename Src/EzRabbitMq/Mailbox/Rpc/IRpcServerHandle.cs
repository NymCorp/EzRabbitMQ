namespace EzRabbitMQ
{
    /// <summary>
    /// Adding an Handle method on your RpcServer to receive IRpcRequest
    /// </summary>
    /// <typeparam name="TResponse">The response type returned for the request TRequest</typeparam>
    /// <typeparam name="TRequest">The request type to receive</typeparam>
    public interface IRpcServerHandle<out TResponse, in TRequest>
        where TResponse : IRpcResponse
        where TRequest : IRpcRequest
    {
        /// <summary>
        /// Handle called when a request is received
        /// </summary>
        /// <param name="request">The request type to receive</param>
        /// <returns>The response type returned for the request TRequest</returns>
        TResponse Handle(TRequest request);
    }

    /// <summary>
    /// Simple implementation of the rpc server handle
    /// </summary>
    public interface IRpcServerHandle : IRpcServerHandle<EmptyRpcResponse, EmptyRpcRequest>
    {
    }
}