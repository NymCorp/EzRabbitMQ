namespace EzRabbitMQ.Tests.Messages
{
    public record RpcIncrementRequest: IRpcRequest
    {
        public int CurrentValue { get; init; }
    }
}