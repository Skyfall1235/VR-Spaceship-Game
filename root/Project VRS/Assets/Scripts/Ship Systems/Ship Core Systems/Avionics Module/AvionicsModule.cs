using UnityEngine;

public class AvionicsModule : BC_CoreModule
{
    protected BC_ShipMovementController movementController;
    protected Rigidbody ShipRigidbody;
    protected BC_ShipInputHandler shipInputHandler;

    //flight data, positional info
    protected Vector3 PositionInSpace => transform.position;
    protected Vector3 AngularVelocity;
    protected Vector3 Velocity;
    protected Vector3 LastVelocity;
    protected float EngineThrust; // the speed of forward
    protected float InertialForce;

    #region Start up and Shut down logic

    protected override void PreStartUpLogic()
    {
        base.PreStartUpLogic();
        ShipRigidbody = transform.root.GetComponent<Rigidbody>();
        shipInputHandler = transform.root.GetComponent<BC_ShipInputHandler>();
    }

    protected override void PostStartUpLogic()
    {
        base.PostStartUpLogic();
        shipInputHandler.ControlsAreResponsive = true;
    }

    protected override void PreShutDownLogic()
    {
        base.PreShutDownLogic();
        shipInputHandler.ControlsAreResponsive = false;
        ShipRigidbody = null;
    }

    protected override void PostShutDownLogic()
    {
        base.PostShutDownLogic();
    }

    #endregion

    #region mathematics

    protected void RetrieveRigidbodyInfo()
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

    protected void CalculateIntertialForce()
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

