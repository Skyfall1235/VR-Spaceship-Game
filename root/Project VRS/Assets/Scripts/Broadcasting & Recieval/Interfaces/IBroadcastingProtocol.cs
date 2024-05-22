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
    /// The timestamp of the last broadcast sent by this entity.
    /// </summary>
    public DateTime LastBroadcastTime { get; }

    /// <summary>
    /// (Protected) The timestamp of this entity's broadcast at the start 
    /// (used for internal calculations). This property can be set.
    /// </summary>
    protected DateTime BroadcastTimeAtStart { get; set; }

    /// <summary>
    /// (Protected) The global start broadcast time for all entities implementing 
    /// this interface (reference point to the start of the application).
    /// </summary>
    protected DateTime GlobalStartBroadcastTime { get; }

    /// <summary>
    /// Updates the LastBroadcastTime property with the provided current time.
    /// </summary>
    /// <param name="currentTime">The current time to be used for update.</param>
    public void UpdateLastBroadcastTime(DateTime currentTime);

    /// <summary>
    /// Initializes the BroadcastTimeAtStart property with the provided current time.
    /// </summary>
    /// <param name="currentTime">The start time of the application to be used for initialization.</param>
    public void InitializeTimeAtStart(DateTime currentTime);

    /// <summary>
    /// Abstract base class representing broadcast data. 
    /// Specific broadcast data types should inherit from this class.
    /// </summary>
    public abstract class BroadcastData { }
}