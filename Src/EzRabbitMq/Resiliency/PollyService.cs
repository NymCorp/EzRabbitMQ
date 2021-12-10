
using System;
using System.Threading.Tasks;

namespace EzRabbitMQ.Resiliency
{
    /// <inheritdoc />
    public class PollyService : IPollyService
    {
        private readonly EzRabbitMQConfig _config;

        /// <summary>
        /// Create the polly service 
        /// </summary>
        /// <param name="config"></param>
        public PollyService(EzRabbitMQConfig config) => _config = config;

        /// <inheritdoc />
        public async Task ExecuteAsync(Func<Task> action)
        {
            await _config.PollyPolicy.ExecuteAsync(action);
        }

        /// <inheritdoc />
        public void Execute(Action action)
        {
            ExecuteAsync(() => RunPolicy(action)).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public void TryExecute<T>(Action action) where T : Exception
        {
            try
            {
                Execute(action);
            }
            catch (Exception e)
            {
                var specificException = Activator.CreateInstance(typeof(T), e) as Exception;
                
                throw specificException ?? e;
            }
        }

        private static Task RunPolicy(Action action)
        {
            return Task.Run(action);
        }
    }
}