using System;

namespace EzRabbitMQ.Tests
{
    public interface IRandomService
    {
        int GetRandomInt();
    }
    
    public class RandomService: IRandomService
    {
        public int GetRandomInt()
        {
            var random = new Random();
            return random.Next(0, 100);
        }
    }
}