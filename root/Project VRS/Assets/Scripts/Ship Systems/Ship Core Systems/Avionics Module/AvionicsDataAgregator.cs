using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AvionicsModule
{
    //flight data, positional info
    Vector3 PositionInSpace;
    Vector3 AngularVelocity;
    Vector3 Velocity;
    Vector3 LastVelocity;
    float EngineThrust; // the speed of forward
    float Throttle;
    float BreakingForce;
    float yawInput;
    float pitchInput;
    float RollInput;
    float InertialForce;

    protected bool AreControlsResponsive = true;

    BC_ShipMovementController movementController;
    Rigidbody ShipRigidbody;

    public override void Awake()
    {
        base.Awake();
    }

    public void FixedUpdate()
    {
        //this should probably be more optimised, but i will just come back to it later
        RetrievePosition();
        RetrieveRigidbodyInfo();
        CalculateIntertialForce();
    }

    #region Data aggregation

    void ParseInput(ShipJoystickInput input)
    {
        Throttle = input.ThrustValue;
        yawInput = input.yawValue;
        RollInput = input.PrimaryFlightStick.x;
        pitchInput = input.PrimaryFlightStick.y;
        BreakingForce = input.BreakValue;
    }

    void RetrievePosition()
    {
        PositionInSpace = transform.position;
    }

    void RetrieveRigidbodyInfo()
    {
        //oV - old velocity, nV - new velocity, aV - angular velocity, mV - magnitude of velocity

        //save oV
        LastVelocity = Velocity;
        //retrieve nV
        Velocity = ShipRigidbody.velocity;
        //retrieve forward moventum
        EngineThrust = Velocity.magnitude;
        //retrieve aV
        AngularVelocity = ShipRigidbody.angularVelocity;
    }

    void CalculateIntertialForce()
    {
        //due later
        const float GravitationalConstant = 9.8f;
        float acceleration = (Velocity - LastVelocity).magnitude;
        float geforceInt = acceleration / GravitationalConstant; //gets the int value
        float geforceFloat = acceleration % GravitationalConstant;//gets the remainder
        float finalIForce = geforceInt + geforceFloat;
        //the accelleration per second divided by the gravitational const, negative is blood to head, positive is blood to feet
        InertialForce = finalIForce;
    }

    #endregion
}
