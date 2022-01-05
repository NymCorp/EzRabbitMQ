namespace EzRabbitMQ.Tests.Messages
{
    public record RpcSampleResponse
    {
        public TestSample Model { get; init; }
    }
}