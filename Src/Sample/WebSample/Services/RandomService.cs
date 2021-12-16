public interface IAdditionService
{
    int Addition(int a, int b);
}

public class AdditionService : IAdditionService
{
    public int Addition(int a, int b) => a + b;
}