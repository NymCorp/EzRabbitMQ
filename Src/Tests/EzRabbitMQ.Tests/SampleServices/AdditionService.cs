namespace EzRabbitMQ.Tests
{
    public interface IAdditionService
    {
        int Add(int a, int b);
    }
    
    public class AdditionService: IAdditionService
    {
        public int Add(int a, int b) => a + b;
    }
}