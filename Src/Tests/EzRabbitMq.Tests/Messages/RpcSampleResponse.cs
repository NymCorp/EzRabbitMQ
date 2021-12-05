namespace EzRabbitMQ.Tests.Messages
{
    public record RpcSampleResponse: IRpcResponse
    {
        public TestSample Model { get; init; }
    }
}