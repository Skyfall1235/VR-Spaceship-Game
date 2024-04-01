using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public partial class MissileBehavior : MonoBehaviour
{
    [Header("Target Data")]
    [SerializeField] SO_MissileData m_behaviorParameters;
    //the target, TARGET TYPE
    [Serializable]
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

    private Rigidbody m_rigidbody;
    private bool ObjectLifeTimeIsFinished = false;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    public void FireMissile(GameObject target, TargetType type)
    {
        m_target = target;
        m_targetType = type;
        //start a coroutine for the flight path?
    }

    protected void OnInterceptOfTarget()
    {
        KillGuidance();
        OnInterceptHit.Invoke();
    }

    protected void OnPassOfTarget()
    {
        KillGuidance();
        OnPassTarget.Invoke();
    }

    //is clled at the end of the corotuine to update the trajectory of the rigidbody
    private void ApplyGuidanceCommand(Vector3 command)
    {

    }
    //now we get to write the actual movement of the missile! yippee!

    private void FixedUpdate()
    {
        if (!ObjectLifeTimeIsFinished)
        {

        }
    }





}
