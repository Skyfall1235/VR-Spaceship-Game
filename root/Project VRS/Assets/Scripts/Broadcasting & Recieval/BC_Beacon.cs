using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a basic beacon with broadcasting capabilities.
/// </summary>
public partial class BC_Beacon : MonoBehaviour, IBeacon
{
    /// <summary>
    /// Maximum distance for detecting accessible beacons.
    /// </summary>
    [SerializeField]
    protected float m_beaconMaxDistance = 1000f;

    /// <summary>
    /// Broadcasting data for the beacon.
    /// </summary>
    [SerializeField]
    private IBeacon.BeaconData m_broadcastingData;
    public IBeacon.BeaconData BroadcastingData { get => m_broadcastingData; }

    #region Static Beacon Data

    /// <summary>
    /// Update interval for beacon data.
    /// </summary>
    const float k_UpdateSecondsInterval = 1f;

    /// <summary>
    /// List of accessible beacons.
    /// </summary>
    [SerializeField]
    public List<IBeacon> m_accessibleBeacons = new List<IBeacon>();

    /// <summary>
    /// Indicates whether the beacon is currently broadcasting.
    /// </summary>
    internal bool IsBroadcasting = false;

    /// <summary>
    /// Static list of all beacons.
    /// </summary>
    [SerializeField]
    protected static List<IBeacon> s_allBeacons = new List<IBeacon>();

    /// <summary>
    /// Getter and setter for all beacons, triggering an update event.
    /// </summary>
    protected List<IBeacon> AllBeacons
    {
        get => s_allBeacons;
        set
        {
            s_updateAccessibleBeacons.Invoke();
            s_allBeacons = value;
        }
    }

    /// <summary>
    /// UnityEvent triggered when accessible beacons need to be updated.
    /// </summary>
    protected internal UnityEvent s_updateAccessibleBeacons = new UnityEvent();


    #endregion

    #region Monobehavior Methods

    protected virtual void Awake()
    {
        m_broadCastingUID = Guid.NewGuid();
    }

    protected virtual void OnEnable()
    {
        IsBroadcasting = true;
        InitializeTimeAtStart(DateTime.Now);
        s_allBeacons.Add(this);
        s_updateAccessibleBeacons.AddListener(UpdateAccessableBeacons);
        //i think that overriding this method will update this action automatically in future implementation
        StartCoroutine(UpdateBeaconData(OnBroadcastUpdate));
    }

    protected virtual void OnDisable()
    {
        IsBroadcasting = false;

        //update the remove the item, then remove the listener
        s_allBeacons.Remove(this);
        s_updateAccessibleBeacons.RemoveListener(UpdateAccessableBeacons);
        //this shouldnt be needed, but ill leave it here in case it does.
        //StopCoroutine(UpdateBeaconData());
    }

    #endregion

    /// <summary>
    /// Updates the list of accessible beacons based on distance to the beacon.
    /// </summary>
    public void UpdateAccessableBeacons()
    {
        /// A temporary list used to store accessible beacons during updates.
        List<IBeacon> accessibleBeaconsTransfer = new List<IBeacon>();

        foreach (IBeacon beacon in s_allBeacons)
        {
            // Calculate the distance between the current beacon and the other beacon.
            float distance = Vector3.Distance(beacon.BroadcastingData.broadcastPosition, transform.position);

            // If the distance is within the maximum beacon distance, add it to the accessible list.
            if (distance < m_beaconMaxDistance)
            {
                accessibleBeaconsTransfer.Add(beacon);
            }
        }

        // Assign the temporary list to the accessible beacons property.
        m_accessibleBeacons = accessibleBeaconsTransfer;
    }

    /// <summary>
    /// Abstract method to be implemented by derived classes for broadcast updates.
    /// </summary>
    protected virtual void OnBroadcastUpdate()
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// Coroutine to update beacon data at regular intervals.
    /// </summary>
    /// <param name="onBroadcastUpdate">Action to be invoked on each update.</param>
    protected IEnumerator UpdateBeaconData(Action onBroadcastUpdate)
    {
        while (IsBroadcasting)
        {
            yield return new WaitForSeconds(k_UpdateSecondsInterval);
            onBroadcastUpdate();
        }
    }

}

public partial class BC_Beacon
{
    /// <summary>
    /// Unique identifier for the broadcasting beacon. This property is hidden in the inspector.
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private Guid m_broadCastingUID;
    public Guid BroadCastingUID => m_broadCastingUID;

    /// <summary>
    /// Broadcast start time for this beacon.
    /// </summary>
    protected DateTime m_broadcastTimeAtStart;

    /// <summary>
    /// Interface implementation for broadcast start time.
    /// </summary>
    DateTime IBroadcastingProtocol.BroadcastTimeAtStart => m_broadcastTimeAtStart;

    /// <summary>
    /// Calculates the number of seconds elapsed since the beacon began broadcasting, 
    /// or returns null if not currently broadcasting.
    /// </summary>
    public float? GetSecondsSinceBeginBroadcast()
    {
        if (!IsBroadcasting)
        {
            CustomLogger.Log("An item is requesting the time since broad cast while the object is not active.", CustomLogger.LogLevel.Warning, CustomLogger.LogCategory.System);
            return null;
        }

        if (m_broadcastTimeAtStart == DateTime.MinValue)
        {
            // Handle case where start time is not initialized (consider a different default value if needed)
            return 0f;
        }

        TimeSpan elapsedTime = DateTime.Now - m_broadcastTimeAtStart;
        return (float)elapsedTime.TotalSeconds;
    }

    /// <summary>
    /// Initializes the broadcast start time with the provided current time.
    /// </summary>
    /// <param name="currentTime">The current time to set as the broadcast start time.</param>
    internal void InitializeTimeAtStart(DateTime currentTime)
    {
        m_broadcastTimeAtStart = currentTime;
    }

    /// <summary>
    /// Sets the faction data for the beacon's broadcasting data.
    /// </summary>
    /// <param name="factionData">The ScriptableObject containing faction data.</param>
    protected void SetFactionData(SO_FactionData factionData)
    {
        m_broadcastingData.m_factionData = factionData;
    }

}

