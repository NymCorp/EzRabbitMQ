using MessagePack;

namespace Benchmark.Models
{
    public interface IRpcIncrementResponse
    {
        int NewValue { get; }
    }
    
    [MessagePackObject]
    public record MsgPackRpcIncrementResponse(int NewValue) : IRpcIncrementResponse
    {
        [Key(0)]
        public int NewValue { get; } = NewValue;
    }

    public record RpcIncrementResponse(int NewValue): IRpcIncrementResponse;
}