using EzRabbitMQ;
using MessagePack;

namespace Benchmark.Models
{
    [MessagePackObject]
    public record RpcIncrementRequest(int CurrentValue): IRpcRequest
    {
        [Key(0)]
        public int CurrentValue { get; set; } = CurrentValue;
    }
}