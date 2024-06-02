using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBeacon : MonoBehaviour, IBeacon
{
    [SerializeField]
    private IBeacon.BeaconData m_broadcastingData;
    public IBeacon.BeaconData BroadcastingData { get => m_broadcastingData; }

    [SerializeField]
    private Guid m_broadCastingUID;
    public Guid BroadCastingUID
    {
        get => m_broadCastingUID;
    }

    [SerializeField]
    private DateTime m_lastBroadcastTime;
    public DateTime LastBroadcastTime
    {
        get => m_lastBroadcastTime;
    }


    DateTime IBroadcastingProtocol.BroadcastTimeAtStart { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


    DateTime IBroadcastingProtocol.GlobalStartBroadcastTime => throw new NotImplementedException();

    public void InitializeTimeAtStart(DateTime currentTime)
    {
        
    }

    public void UpdateLastBroadcastTime(DateTime currentTime)
    {

    }
}
