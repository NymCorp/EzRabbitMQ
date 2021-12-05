using System.Collections.Generic;

namespace EzRabbitMQ
{
    /// <summary>
    /// Queue options.
    /// </summary>
    public interface IQueueOptions
    {
        /// <summary>
        ///     When durable is set to true the queue will persist after server restart.
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        ///     <value>true</value>
        /// </summary>
        bool QueueDurable { get; }

        /// <summary>
        ///     Enable auto delete feature. <br/>
        ///     From RabbitMQ documentation :
        ///     <i>
        ///         If set, the queue is deleted when all consumers have finished using it. 
        ///         The last consumer can be cancelled either explicitly or because its channel is closed.  
        ///         If there was no consumer ever on the queue, it won't be deleted. 
        ///         Applications can explicitly delete auto-delete queues using the Delete method as normal.
        ///     </i>
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        ///     <value>false</value>
        /// </summary>
        bool QueueAutoDelete { get; }

        /// <summary>
        ///     If AckMultiple acknowledge is set to <b>true</b> the every message up to the last one ack will be acknowledge.
        ///     <value>false</value>
        /// </summary>
        bool AckMultiple { get; }

        /// <summary>
        ///     If Queue is exclusive only one consumer will be able to connect at time.
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        ///     <value>false</value>
        /// </summary>
        bool QueueExclusive { get; }

        /// <summary>
        ///     Set the maximum number of message stored inside the queue.
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b><br/>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        ///     <value>0 = no limit</value>
        /// </summary>
        int QueueSizeLimit { get; }

        /// <summary>
        ///     Set the exchange target of the dead letter messages.
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b><br/>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        ///     <value>null</value>
        /// </summary>
        string? DeadLetterExchangeName { get; }

        /// <summary>
        ///     Set the routing key target of the dead Letter messages.
        ///     <remarks>
        ///         Change this field will create a server breaking change and <b>needs a queue recreation.</b><br/>
        ///         To <b>automatically try to recreate</b> the queue you can change the <see cref="QueueRecreateMode"/>
        ///         and set the enum flag value to <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        ///         or <see cref="RecreateMode.ForceRecreate"/>.
        ///         You can also delete the queue manually and the system will redeclare it.
        ///     </remarks>
        /// <value>null</value>
        /// </summary>
        string? DeadLetterRoutingKey { get; }

        /// <summary>
        /// Set the queue max priority <b>for the current consumer binding.</b>
        /// <value>null</value>
        /// </summary>
        public byte? QueueMaxPriority { get; }

        /// <summary>
        /// Set the queue mode (default, lazy, or quorum).
        /// <value>QueueMode.Default</value>
        /// </summary>
        QueueMode QueueMode { get; }
        
        /// <summary>
        /// Set the recreate behavior of the queue, you can force recreate using <see cref="RecreateMode.ForceRecreate"/>
        /// You can also recreate the queue only if a <b>breaking change is detected</b> using <see cref="RecreateMode.RecreateIfBreakingChangeDetected"/>
        /// You can also set this flag to <see cref="RecreateMode.RecreateIfEmpty"/> to prevent queue recreation on not empty queue.
        /// <value><see cref="RecreateMode.None"/></value>
        /// </summary>
        RecreateMode QueueRecreateMode { get; }

        /// <summary>
        /// Queue declare arguments
        /// </summary>
        Dictionary<string, object> QueueDeclareArguments { get; }

        /// <summary>
        /// Queue binding arguments
        /// </summary>
        Dictionary<string, object> QueueBindArguments { get; }
    }
}