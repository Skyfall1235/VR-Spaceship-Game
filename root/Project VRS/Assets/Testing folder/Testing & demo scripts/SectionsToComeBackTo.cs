using UnityEngine;

//This script is an archive of solutions that were put on pause for any reason.

//please region each section, state the resond for archine,

public class SectionsToComeBackTo
{

    ///REASON FOR ARCHIVE - too compilcated for the prototype, opted for a non mass invived force apl;liucation and high normal and angular drag to replicate. 
    ///ORIGINAL LOCATION - Ship Movement Controller
    #region application of Deceleration

    [Range(0f, 1f)]
    public float DampeningMultiplier = 0.75f;

    //this section is to help slow down the RB
    //we can add if statements to the call updates above to run these modules

    //void BrakeLinearMovement()
    //{
    //    //get the trigger value from the m_hand that is currently grabbing the joystick
    //    Vector3 currentLinearVector = savedForwardVector;
    //    ShipJoystickInput currentInput = GrabCurrentShipControlInputs();

    //    //get the current trigger value
    //    float triggerVal = currentInput.BreakValue;
    //    //set the consts
    //    const float minTriggerVal = 0f;
    //    const float maxTriggerVal = 1f;

    //    //remap the trigger value to be equal to the breakforce to be applied against the linear motion
    //    float mappedTriggerVal = Remap(triggerVal, minTriggerVal, maxTriggerVal, 0f, MaxBreakValue);

    //    //apply a force opposite of the current vector normalized, multiplied by the trigger val
    //    Vector3 BreakingVector = (-currentLinearVector.normalized * mappedTriggerVal);
    //    ApplyLinearMotionValue(BreakingVector);
    //}

    ////will be needed for when the throttle is at zero
    //void DecelerateLinearMovement()
    //{
    //    //determine inverted force needed on the fly based on the current linear velocity
    //    Vector3 currentInputVector = savedForwardVector;
    //    Vector3 invertedDampendVector = -currentInputVector * DampeningMultiplier;
    //    ApplyLinearMotionValue(invertedDampendVector);
    //}

    //void DecelerateRotationalMovement()
    //{
    //    //determine inverted torque needed on the fly based on the current torque
    //    Vector3 currentTorque = savedRotationAxis;
    //    float currentRotationalSpeed = savedRotationSpeed;

    //    //invert the vector3 and apply it when input is equal to zero, so it stops the rotation of whereever it is
    //    Vector3 invertedTorque = -currentTorque;
    //    ApplyRotationOnAxis(invertedTorque * DampeningMultiplier, -currentRotationalSpeed * DampeningMultiplier);//this might be problematic, but it think i just need to invert the rotational speed?
    //}

    //void DecelerateStrafe()
    //{
    //    //determine inverted force needed on the fly based on the current strafe velocity
    //    Vector3 currentStrafe = savedStrafeVector;
    //    Vector3 invertedStrafe = -currentStrafe;
    //    ApplyStrafe(invertedStrafe * DampeningMultiplier);
    //}

    #endregion

    ///REASON FOR ARCHIVE - was only really useful in very particular cases.
    ///ORIGINAL LOCATION - ship input handler
    #region -2->2 to -1->1 converter
    float ConvertToSignedRange(float value)
    {
        float convertedNum = (value * 2) - 1;
        return convertedNum;
    }

    #endregion
}
