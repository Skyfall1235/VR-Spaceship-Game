using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public partial class AvionicsModule
{
    //flight data, positional info
    Vector3 PositionInSpace;
    Vector3 AngularVelocity;
    Vector3 Velocity;
    Vector3 LastVelocity;
    float EngineThrust; // the speed of foward
    float Throttle;
    float yawInput;
    float pitchInput;
    float RollInput;
    float InertialForce;

    bool AreControlsResponsive = true;

    BC_ShipMovementController movementController;
    Rigidbody ShipRigidbody;

    public override void Awake()
    {
        base.Awake();
        ShipRigidbody = transform.root.GetComponent<Rigidbody>();
        movementController.OnUpdateInputs.AddListener(ParseInput);
        movementController.OnUpdateInputs.AddListener(ParseInput);
    }

    public void FixedUpdate()
    {
        RetrievePosition();
        RetrieveRigidbodyInfo();
    }

    void ParseInput(ShipJoystickInput input)
    {
        Throttle = input.ThrustValue;
        yawInput = input.yawValue;
        RollInput = input.PrimaryFlightStick.x;
        pitchInput = input.PrimaryFlightStick.y;
    }

    void RetrievePosition()
    {
        PositionInSpace = transform.position;
    }

    void RetrieveRigidbodyInfo()
    {
        Velocity = ShipRigidbody.velocity;
        AngularVelocity = ShipRigidbody.angularVelocity;
    }

    void CalculateIntertialForce()
    {
        //due later
        const float GravitationalConstant = 9.8f;
        float accelleration = (Velocity - LastVelocity).magnitude;
        float GeforceInt = accelleration / GravitationalConstant;
        float GeforceFloat = accelleration % GravitationalConstant;
        //the accelleration per second divided by the gravitational const, negative is blood to head, positive is blood to feet
    }
}
