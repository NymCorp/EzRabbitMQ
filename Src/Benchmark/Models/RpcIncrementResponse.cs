using EzRabbitMQ;
using MessagePack;

namespace Benchmark.Models
{
    [MessagePackObject]
    public record RpcIncrementResponse(int NewValue): IRpcResponse
    {
        [Key(0)]
        public int NewValue { get; set; } = NewValue;
    }
}