namespace EzRabbitMQ.Tests.Messages
{
    public record RpcSampleRequest
    {
        public string Text { get; init; }
    }
}