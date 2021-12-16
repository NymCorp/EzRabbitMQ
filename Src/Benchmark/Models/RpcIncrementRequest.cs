using MessagePack;

namespace Benchmark.Models
{
    public interface IRpcIncrementRequest
    {
        int CurrentValue { get; }
    }

    public record RpcIncrementRequest(int CurrentValue) : IRpcIncrementRequest;

    [MessagePackObject]
    public record MsgPackRpcIncrementRequest(int CurrentValue) : IRpcIncrementRequest
    {
        [Key(0)]
        public int CurrentValue { get; } = CurrentValue;
    }
}