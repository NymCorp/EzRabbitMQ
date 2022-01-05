namespace EzRabbitMQ.Resiliency;

/// <summary>
/// Polly service used for retry policies
/// </summary>
public interface IPollyService
{
    /// <summary>
    /// Execute action async
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <returns>Return an awaitable task</returns>
    public Task ExecuteAsync(Func<Task> action);

    /// <summary>
    /// Execute action sync
    /// </summary>
    /// <param name="action">Action to execute</param>
    public void Execute(Action action);

    /// <summary>
    /// Try execute action the raise a custom exception
    /// </summary>
    /// <param name="action">Action to execute</param>
    /// <typeparam name="T">Exception type you want to throw on action exception</typeparam>
    public void TryExecute<T>(Action action) where T : Exception;
}