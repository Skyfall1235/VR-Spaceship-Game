using System;

/// <summary>
/// Interface representing a broadcasting protocol.
/// </summary>
public interface IBroadcastingProtocol
{
    /// <summary>
    /// Unique identifier for the broadcasting entity.
    /// </summary>
    public Guid BroadCastingUID { get; }

    /// <summary>
    /// (Protected) The timestamp of this entity's broadcast at the start 
    /// (used for internal calculations). This property can be set.
    /// </summary>
    protected DateTime BroadcastTimeAtStart { get; }

    /// <summary>
    /// Calculates the time between beacon initialization and the current time.
    /// </summary>
    /// <returns>A float value representing the number of seconds that have elasped since the beacon began boradcasting.</returns>
    public float? GetSecondsSinceBeginBroadcast();

    /// <summary>
    /// Abstract base class representing broadcast data. 
    /// Specific broadcast data types should inherit from this class.
    /// </summary>
    public abstract class BroadcastData { }
}