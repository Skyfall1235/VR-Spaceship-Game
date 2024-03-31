using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public partial class MissileBehavior : MonoBehaviour
{
    [Header("Target Data")]
    [SerializeField] SO_MissileData behaviorParameters;
    //the target, TARGET TYPE
    public enum TargetType
    {
        Stationary,
        Dynamic
    }

    [SerializeField]
    private TargetType m_targetType;
    [SerializeField]
    private GameObject m_target;

    [Header("Navagation")]
    public float NavigationGain; // Adjust this value to tune guidance aggressiveness

    [Header("Events")]
    public UnityEvent<int> OnStageFinish = new();
    public UnityEvent OnInterceptHit = new();
    public UnityEvent OnPassTarget = new();


    public void FireMissile(GameObject target, TargetType type)
    {
        m_target = target;
        m_targetType = type;
        //start a coroutine for the flight path?
    }


    //AN INTERCEPT EVENT
    //an event that gets called with each stage getting used up so we can do cool stuff with it down the road?
    //also a passed target event
}
