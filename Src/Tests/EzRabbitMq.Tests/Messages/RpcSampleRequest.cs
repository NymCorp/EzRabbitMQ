namespace EzRabbitMQ.Tests.Messages
{
    public record RpcSampleRequest: IRpcRequest
    {
        public string Text { get; init; }
    }
}