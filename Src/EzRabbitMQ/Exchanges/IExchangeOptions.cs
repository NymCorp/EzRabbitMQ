namespace EzRabbitMQ;

/// <summary>
/// Exchange Options
/// </summary>
public interface IExchangeOptions
{
    /// <summary>
    /// If set to true the exchange will persist after the server restart
    ///     <remarks>
    ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b><br/>
    ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="ExchangeRecreateMode"/>
    ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
    ///         or <see cref="RecreateMode.ForceRecreate"/>.
    ///         You can also delete the queue manually and the system will redeclare it.
    ///     </remarks>
    /// <value>false</value>
    /// </summary>
    bool ExchangeDurable { get; }

    /// <summary>
    /// If Auto delete is set to true the exchange will delete itself after the last disconnection <br/>
    /// If no connection the exchange can stay create
    /// Default: <c>false</c>
    /// </summary>
    bool ExchangeAutoDelete { get; }

    /// <summary>
    /// Set the recreate behavior of the queue, you can force recreate using <see cref="RecreateMode.ForceRecreate"/>
    /// You can also recreate the exchange only if a <b>breaking change is detected</b> using <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
    /// You can also set this flag to <see cref="RecreateMode.RecreateIfEmpty"/> to prevent exchange recreation on not empty queue.
    /// <value><see cref="RecreateMode.None"/></value>
    /// </summary>
    RecreateMode ExchangeRecreateMode { get; }

    /// <summary>
    /// Dictionary holding the exchange arguments, some argument change needs a exchange deletion
    /// </summary>
    Dictionary<string, object> ExchangeDeclareArguments { get; }
}