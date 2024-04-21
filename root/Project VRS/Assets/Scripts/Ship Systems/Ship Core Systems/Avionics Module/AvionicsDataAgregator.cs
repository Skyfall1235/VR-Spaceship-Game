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
    }
}
