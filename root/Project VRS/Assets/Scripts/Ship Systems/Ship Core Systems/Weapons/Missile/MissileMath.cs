using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public partial class MissileBehavior 
{

    public Vector3 CalculateGuidanceCommand(Transform target)
    {
        // 1. Get positions and velocities
        Vector3 missilePosition = transform.position;
        Vector3 targetPosition = target.position;
        Vector3 missileVelocity = transform.GetComponent<Rigidbody>().velocity;
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;

        // 2. Calculate relative position and velocity
        Vector3 relativePosition = targetPosition - missilePosition;
        Vector3 relativeVelovicity = targetVelocity - missileVelocity;

        // 3. Line-Of-Sight (LOS) calculations
        
        Vector3 LOSRate = CalculateLOSRate(relativePosition, relativeVelovicity);

        // 4. Target Acceleration Estimation (omitted for simplicity)
        // In a real implementation, estimate target acceleration (m_targetAcc) using filtering techniques.

        // 5. APN Guidance Command
        Vector3 propTerm = NavigationGain * LOSRate;
        Vector3 guidanceCommand = propTerm; // No target acceleration estimation in this example

        return guidanceCommand;
    }

    private Vector3 CalculateLOSRate(Vector3 relativePosition, Vector3 relativeVelocity)
    {
        // This function needs to be implemented based on calculus or numerical differentiation 
        //EDIT: we will use calculus because its more precise and generally avoids rounding errors
        Vector3 LOS = relativePosition.normalized;
        Vector3 LOSRate = Vector3.Cross(relativeVelocity, LOS) / Mathf.Pow(relativePosition.magnitude, 2);

        //  to find the time derivative of the LOS vector.
        return LOSRate;
    }
}
