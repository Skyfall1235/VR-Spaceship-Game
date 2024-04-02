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

    #region Monobehavior Methods
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }
    //now we get to write the actual movement of the missile! yippee!

    private void FixedUpdate()
    {
        //both checks to ensure the call should run and that the next calculation is *ready*
        if (!ObjectLifeTimeIsFinished && m_CorotuineFinishFlag)
        {
            m_CorotuineFinishFlag = false;
            m_guidanceCommandLoop = StartCoroutine(ComputeAndExecuteGuidanceCommand());
            m_rigidbody.AddRelativeForce(Vector3.forward * 100);
            //now, add a rotational force 
            m_rigidbody.AddRelativeTorque(-guidanceVector);
            //reset the force so it doesnt get applied additively
            guidanceVector = Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        //draw a ray in direction of the guidnce command from the missile
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + (guidanceVector);
        Debug.DrawLine(startPoint, endPoint, Color.green);
    }

    private void OnDestroy()
    {
        //Stop Coroutine the coroutine
        if (m_guidanceCommandLoop != null)
        {
            StopCoroutine(m_guidanceCommandLoop);
        }
        //force a completion of the last step, then kill the native array AFTER. (i think this avoids a memory leak?)
        m_guidanceCommandJob.Complete();
        //memoryAllocation.Dispose();
    }
    #endregion

    #region Launch control
    public void StartLaunchSequence()
    {

    }

    public void TrackTarget()
    {

    }
    public void Launch()
    {

    }

    #endregion


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
    



}
