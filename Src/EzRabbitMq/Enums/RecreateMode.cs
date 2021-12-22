namespace EzRabbitMQ;

/// <summary>
/// Queue Recreate Mode will trigger a queue recreate following flags rule
/// </summary>
[Flags]
public enum RecreateMode
{
    /// <summary>
    /// No queue recreate
    /// </summary>
    None = 0,

    /// <summary>
    /// Queue will be recreate if unused
    /// </summary>
    RecreateIfUnused = 1 << 1,

    /// <summary>
    /// Queue will be recreate if empty
    /// </summary>
    RecreateIfEmpty = 1 << 2,

    /// <summary>
    /// Queue will be recreate anyway, ignoring IfUnused and IfEmpty
    /// </summary>
    ForceRecreate = 1 << 3,

    /// <summary>
    /// Queue will be recreate if configure queue detected features changes
    /// </summary>
    RecreateIfBreakingChangeDetected = 1 << 4
}