using RabbitMQ.Client.Exceptions;

namespace EzRabbitMQ.Exceptions;

/// <summary>
/// Provide helper for RabbitMQ exceptions
/// </summary>
public static class RabbitMQExceptionInterceptor
{
    /// <summary>
    /// Parse RabbitMQ Exception to return enum type of the inner exception.
    /// </summary>
    /// <param name="e">RabbitMQ exception</param>
    /// <returns>Handled RabbitMQ type</returns>
    // ReSharper disable once InconsistentNaming
    public static RabbitMQExceptionType ParseRabbitMQException(Exception e)
    {
        if (e is OperationInterruptedException op)
        {
            if (op.Message.Contains("inequivalent arg"))
            {
                return RabbitMQExceptionType.InequivalentArg;
            }

            if (op.Message.Contains("code=404, text='NOT_FOUND - no queue"))
            {
                return RabbitMQExceptionType.QueueNotFound;
            }

            if (op.Message.Contains("code=404, text='NOT_FOUND - no exchange"))
            {
                return RabbitMQExceptionType.ExchangeNotFound;
            }

            if (op.Message.Contains("code=405, text='RESOURCE_LOCKED - cannot obtain exclusive access to locked queue"))
            {
                return RabbitMQExceptionType.QueueIsExclusiveAndAlreadyExists;
            }
        }

        return RabbitMQExceptionType.Unknown;
    }
}