namespace EzRabbitMQ.Exceptions;

/// <summary>
/// Publish exception
/// </summary>
public class PublishException : Exception
{
    /// <summary>
    /// Publish exception
    /// </summary>
    public PublishException(Exception e)
        : base("Unable to publish message from producer", e)
    {
    }
}

/// <summary>
/// Create channel exception
/// </summary>
public class CreateChannelException : Exception
{
    /// <summary>
    /// Create channel exception
    /// </summary>
    public CreateChannelException(Exception e)
        : base("Unable to create session channel", e)
    {
    }
}

/// <summary>
/// Create Connection Exception
/// </summary>
public class CreateConnectionException : Exception
{
    /// <summary>
    /// Create Connection Exception
    /// </summary>
    public CreateConnectionException(Exception e)
        : base("Unable to connect after multiple tries. Please verify settings", e)
    {
    }
}

/// <summary>
/// Create consumer exception
/// </summary>
public class CreateConsumerException : Exception
{
    /// <summary>
    /// Create consumer exception
    /// </summary>
    public CreateConsumerException(Exception e)
        : base("Unable to create consumer", e)
    {
    }
}

/// <summary>
/// Configure mailbox exception
/// </summary>
public class ConfigureMailboxException : Exception
{
    /// <summary>
    /// Configure mailbox exception
    /// </summary>
    public ConfigureMailboxException(Exception e)
        : base($"Unable to configure mailbox : {e.Message}", e)
    {
    }
}

/// <summary>
/// Exception raised when an obsolete event message is received or a deleted of renamed type
/// </summary>
public class ReflectionNotFoundTypeException : Exception
{
    /// <summary>
    /// Exception raised when an obsolete event message is received or a deleted of renamed type
    /// </summary>
    public ReflectionNotFoundTypeException(string typeName)
        : base($"Unable to find matching type in the assembly for type : {typeName}." +
               "This can occured if the type has been renamed, or deleted.")
    {
    }
}

/// <summary>
/// Exception raised when an obsolete event message is received or a deleted of renamed type
/// </summary>
public class ReflectionNotFoundHandleException : Exception
{
    /// <summary>
    /// Exception raised when an obsolete event message is received or a deleted of renamed type
    /// </summary>
    public ReflectionNotFoundHandleException(string typeName, string handleName, string paramType)
        : base($"Unable to find handle '{handleName}' in type : {typeName} taking parameter type : {paramType}." +
               "This can occured if the type has been renamed, or deleted." +
               $"This can occured if you forgot to implement {nameof(IMailboxHandler<EmptyRpcRequest>)} in mailbox or " +
               $"if you forgot to implement {nameof(IRpcServerHandle<EmptyRpcResponse, EmptyRpcRequest>)} in rpc server implementation"
        )
    {
    }
}

/// <summary>
/// Exception thrown when a response doesn't match the expected response type
/// </summary>
public class RpcClientCastException : Exception
{
    /// <summary>
    /// Exception thrown when a response doesn't match the expected response type
    /// </summary>
    public RpcClientCastException(Type responseType)
        : base($"Unable to convert received message type to type: {responseType.Name}")
    {
        if (responseType == null) throw new ArgumentNullException(nameof(responseType));
    }
}

/// <summary>
/// Exception thrown when a breaking change is detected in the queue declaration or the exchange declaration
/// </summary>
public class BreakingChangeDetectedException : Exception
{
    /// <summary>
    /// Exception thrown when a breaking change is detected in the queue declaration or the exchange declaration
    /// </summary>
    public BreakingChangeDetectedException() :
        base("Unable to declare the mailbox due to a breaking change." +
             $"Please consider using {nameof(ConsumerOptions.QueueRecreateMode)} =  {nameof(RecreateMode.RecreateIfBreakingChangeDetected)} and/or" +
             $"{nameof(ConsumerOptions.ExchangeRecreateMode)} =  {nameof(RecreateMode.RecreateIfBreakingChangeDetected)}.")
    {
    }
}

/// <summary>
/// Exception thrown when a breaking change is detected in the queue declaration or the exchange declaration
/// and queue deletion and re creation is failing
/// </summary>
public class UnableToRecreateResourceException : Exception
{
    /// <summary>
    /// Exception thrown when a breaking change is detected in the queue declaration or the exchange declaration
    /// and queue deletion and re creation is failing
    /// </summary>
    /// <param name="e">Inner exception detected</param>
    public UnableToRecreateResourceException(Exception e)
        : base("Unable to delete and recreate queue or exchange, see the inner exception", e)
    {
    }
}