using EzRabbitMQ.Exceptions;

namespace EzRabbitMQ.DeletableResource;

/// <summary>
/// Contains base method and implementation for deletable resources
/// </summary>
public abstract class DeletableResourceBase
{
    /// <summary>
    /// Set to true is breaking change is detected
    /// </summary>
    protected bool BreakingChangeDetected;

    private readonly ILogger _logger;
    private readonly ISessionService _sessionService;

    private readonly RecreateMode _recreateMode;
    // private readonly ConsumerOptions _consumerOptions;

    /// <summary>
    /// Deletable resource base, handle create and delete basics
    /// </summary>
    /// <param name="logger">Logger used to log info.</param>
    /// <param name="sessionService">Current session.</param>
    /// <param name="recreateMode"></param>
    protected DeletableResourceBase(ILogger logger, ISessionService sessionService, RecreateMode recreateMode)
    {
        _logger = logger;
        _sessionService = sessionService;
        _recreateMode = recreateMode;
    }

    /// <summary>
    /// Check the consumer options to see if we need to force recreate the current resource
    /// </summary>
    protected void DeleteResourceIfNeeded()
    {
        if (_recreateMode
            is RecreateMode.None
            or RecreateMode.RecreateIfBreakingChangeDetected) return;

        _sessionService.RefreshChannelIfClosed();

        DeleteResource(false, false);
    }

    /// <summary>
    /// Check the configuration to see if we need to delete the current resource on breaking change detected
    /// </summary>
    protected void DeleteResourceOnBreakingChangeIfNeeded()
    {
        if (_recreateMode is RecreateMode.None) return;

        var ifUnused = _recreateMode is RecreateMode.RecreateIfUnused;
        var ifEmpty = _recreateMode is RecreateMode.RecreateIfEmpty;

        _sessionService.RefreshChannelIfClosed();

        DeleteResource(ifUnused, ifEmpty);
    }

    /// <summary>
    /// Handle <see cref="RabbitMQExceptionType.InequivalentArg"/> exception to delete resource if needed
    /// </summary>
    /// <param name="e">Thrown exception.</param>
    /// <exception cref="Exception"></exception>
    protected void HandleException(Exception e)
    {
        var exceptionType = RabbitMQExceptionInterceptor.ParseRabbitMQException(e);

        if (exceptionType is RabbitMQExceptionType.InequivalentArg)
        {
            if (BreakingChangeDetected)
            {
                _logger.LogError(e, "Unable to recreate the resource");
                throw new UnableToRecreateResourceException(e);
            }

            BreakingChangeDetected = true;

            CheckIfRecreateIsEnable();
        }
        else
        {
            _logger.LogError(e, "Un handled exception thrown while trying to create resource");
        }
    }

    private void CheckIfRecreateIsEnable()
    {
        if (_recreateMode is RecreateMode.RecreateIfBreakingChangeDetected)
        {
            DeleteResourceOnBreakingChangeIfNeeded();

            CreateResource();
        }
        else
        {
            throw new BreakingChangeDetectedException();
        }
    }

    /// <summary>
    /// Delete the implemented resource.
    /// </summary>
    /// <param name="ifUnused">Delete only if the resource is unused.</param>
    /// <param name="ifEmpty">Delete only if the resource is empty <b>Only for Queue.</b></param>
    protected abstract void DeleteResource(bool ifUnused, bool ifEmpty);

    /// <summary>
    /// Create the implemented resource.
    /// </summary>
    protected abstract void CreateResource();
}