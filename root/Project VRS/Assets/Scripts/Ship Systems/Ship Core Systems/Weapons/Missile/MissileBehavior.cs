using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public partial class MissileBehavior : MonoBehaviour
{
    [Header("Target Data")]
    [SerializeField] SO_MissileData m_behaviorParameters;
    //the target, TARGET TYPE
    

    [SerializeField]
    private SO_MissileData.TargetType m_targetType;
    [SerializeField]
    private GameObject m_target;

    [Header("Navagation")]
    public float NavigationGain; // Adjust this value to tune guidance aggressiveness

    [Header("Events")]
    public UnityEvent<int> OnStageFinish = new();
    public UnityEvent OnInterceptHit = new();
    public UnityEvent OnPassTarget = new();

    private Rigidbody m_missileRigidbody;
    private bool ObjectLifeTimeIsFinished = false;
    private int currentStage = 0;

    #region Monobehavior Methods
    private void Awake()
    {
        m_missileRigidbody = GetComponent<Rigidbody>();
        OnStageFinish.Invoke(2);
    }
    //now we get to write the actual movement of the missile! yippee!

    private void FixedUpdate()
    {
        //both checks to ensure the call should run and that the next calculation is *ready*
        if (!ObjectLifeTimeIsFinished && m_CorotuineFinishFlag)
        {
            m_CorotuineFinishFlag = false;
            m_guidanceCommandLoop = StartCoroutine(ComputeAndExecuteGuidanceCommand());



            //TESTING

            //m_rigidbody.AddRelativeForce((transform.forward + guidanceVector) * 10);
            //m_rigidbody.MovePosition(force);
            m_missileRigidbody.AddRelativeForce(transform.forward * 10, ForceMode.Acceleration);

            //now, add a rotational force 
            // Define the desired turn axis (e.g., Vector3.up for yaw)
            Vector3 turnAxis = Vector3.up;

            // Calculate torque based on guidance command and desired turn axis
            Vector3 torque = Vector3.Cross(Vector3.up, guidanceVector);
            Vector3 torque2 = Vector3.Cross(Vector3.right, guidanceVector);

            // Apply torque to the Rigidbody
            m_missileRigidbody.AddTorque(torque);
            m_missileRigidbody.AddTorque(torque2);
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
        memoryAllocation.Dispose();
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
        StartCoroutine(MotorStage(0));
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

    private IEnumerator StationaryLaunchSequence()
    {
        //stationary implovies a horizontal launch, so it has to fly up and turn
        return null;
    }

    private IEnumerator DynamicLaunchSequence()
    {
        //dynamic means its forward facing and is likely already moving, like with a ship

        return null;
    }

    private IEnumerator MotorStage(int stage)
    {
        //retrive the limit time
        SO_MissileData.Stage currentStageData = m_behaviorParameters.stages[stage];
        float limitTime = currentStageData.LimitTime;

        float elapsedTime = 0.0f;

        while (elapsedTime < limitTime)
        {
            // Simulate fixed update behavior 


            //the speed based on the motor


            //apply guidance if we use guidance
            if(currentStageData.UseGuidance)
            {

            }



            //add our time and wait for the next fixed update
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //wneh finished, increment and call stage pass
        OnStageFinish.Invoke(currentStage);
        currentStage++;
    }


    //is clled at the end of the corotuine to update the trajectory of the rigidbody
    private void ApplyGuidanceCommand(Vector3 command)
    {

    }
    
    private void ApplyMotorForceByType(SO_MissileData.MotorType motorType, float speed)
    {
        ForceMode forceType = ForceMode.Force;

        switch (motorType)
        {
            case SO_MissileData.MotorType.ConstantSpeed:
                forceType = ForceMode.Force;


                break;
            case SO_MissileData.MotorType.Acceleration:
                forceType = ForceMode.Acceleration;


                break;
            case SO_MissileData.MotorType.None:
                //apply no force and end
                return;
        }
        //calculate direction of force
        Vector3 forwardSpeed = transform.forward * speed;
        //application of the force
        m_missileRigidbody.AddRelativeForce(forwardSpeed, forceType);
    }



}
