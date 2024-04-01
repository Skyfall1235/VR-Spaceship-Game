using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MISSILETEST : MonoBehaviour
{
    //needed parameters for the mathematics
    float3 missilePosition;
    float3 targetPosition;
    float3 missileVelocity;
    float3 targetVelocity;
    [SerializeField] private GameObject m_target;
    //the gain of the course correction
    public float navGain;
    float3 output;
    //the location that we need to story the memory
    [WriteOnly] NativeArray<float3> guidanceCommand;
    // Start is called before the first frame update
    void Update()
    {
        NativeArray<float3> memoryAllocation = new NativeArray<float3>(1, Allocator.TempJob);
        Vector3 missilePosition = transform.position;
        Vector3 targetPosition = m_target.transform.position;
        Vector3 missileVelocity = transform.GetComponent<Rigidbody>().velocity;
        Vector3 targetVelocity = m_target.GetComponent<Rigidbody>().velocity;
        Setup(missilePosition, targetPosition, missileVelocity, targetVelocity, navGain, memoryAllocation);
        output = CalculateGuidanceCommand();
        Debug.Log(output);
        Rigidbody curRB = GetComponent<Rigidbody>();
        curRB.AddRelativeForce(Vector3.forward * 1);
        float Xaxis = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * (Xaxis * 5f));
    }

    public void Setup(Vector3 mP, Vector3 tP, Vector3 mV, Vector3 tV, float nG, NativeArray<float3> nativeArrayReference)
    {
        missilePosition = Vector3ToFloat3(mP);
        targetPosition = Vector3ToFloat3(tP);
        missileVelocity = Vector3ToFloat3(mV);
        targetVelocity = Vector3ToFloat3(tV);
        navGain = nG;
        guidanceCommand = nativeArrayReference;
        //we need the value, but we plan to fill it later so dont bother with it right now
    }
    private float3 CalculateGuidanceCommand()
    {
        
        float3 relativePosition = targetPosition - missilePosition;
        Debug.Log("relative position " + relativePosition);
        //fix in a moment
        float3 relativeVelovicity = targetVelocity - missileVelocity;
        Debug.Log("relative velocity " + relativeVelovicity);

        // 3. Line-Of-Sight (LOS) calculations

        float3 LOSRate = CalculateLOSRate(relativePosition, relativeVelovicity);
        Debug.Log("LOS rate " + LOSRate);

        // 4. Target Acceleration Estimation (omitted for simplicity)
        //EDIT: this can be implemented, but doing so would hurt my brain, so we will continue t omit until nessicary

        // 5. APN Guidance Command
        float3 propTerm = navGain * LOSRate;
        Debug.Log("propterm " + propTerm);
        float3 guidanceCommand = propTerm; // No target acceleration estimation in this example
        Debug.Log("guidance command " + guidanceCommand);
        return guidanceCommand;
    }

    private Vector3 CalculateLOSRate(Vector3 relativePosition, Vector3 relativeVelocity)
    {
        // This function needs to be implemented based on calculus or numerical differentiation 
        //EDIT: we will use calculus because its more precise and generally avoids rounding errors
        float3 LOS = relativePosition.normalized;
        float3 LOSRate = Vector3.Cross(relativeVelocity, LOS) / Mathf.Pow(relativePosition.magnitude, 2.0f);

        //  to find the time derivative of the LOS vector.
        return LOSRate;
    }

    public static float3 Vector3ToFloat3(Vector3 from)
    {
        return new float3(from.x, from.y, from.z);
    }
    public static Vector3 Float3ToVector3(float3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }

    private void OnDrawGizmosSelected()
    {
        //draw a ray in direction of the guidnce command from the missile
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + Float3ToVector3(output * 500);
        Debug.DrawLine(startPoint, endPoint, Color.red);
    }
}
