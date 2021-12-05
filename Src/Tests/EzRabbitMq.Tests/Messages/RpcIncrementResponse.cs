namespace EzRabbitMQ.Tests.Messages
{
    public record RpcIncrementResponse: IRpcResponse
    {
        public int NewValue { get; init; }
    }
}