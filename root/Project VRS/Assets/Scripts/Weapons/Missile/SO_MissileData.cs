using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Simple Missile")]
[Serializable]
public class SO_MissileData : ScriptableObject
{
    public LaunchProperties launchProperties = new LaunchProperties();
    public List<Stage> stages = new List<Stage>();
    public InterceptProperties interceptProperties = new InterceptProperties();

    [Serializable]
    public enum MotorType
    {
        ConstantSpeed,
        Acceleration,
        None
    }

    [Serializable]
    public enum LaunchMode
    { 
        Vertical,
        Horizontal,
        Forward,
        Custom
    }
    [Serializable]
    public struct LaunchProperties
    {
        public LaunchMode LaunchMode;
        public bool AutoMaticLaunch;
        public float trackingTime;
        public float LaunchSpeed;
    }
    [Serializable]
    public struct Stage
    {
        //name
        public string StageName;
        public MotorType MotorType;
        public float limitTime;
        //the acceleration, the use of guidance, type of motor
        public float Speed;
        public bool UseGuidance;
        
    }
    [Serializable]
    public struct InterceptProperties
    {
        public float HitRange;
        public bool StopGuidanceAfterIntercept;
        public bool DestroyMissileOnHit;
    }
    
    

}
