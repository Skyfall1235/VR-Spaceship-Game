using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipInputHandler))]
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementController : MonoBehaviour
{
    /// <summary>
    /// The input handler of the entire ship
    /// </summary>
    private ShipInputHandler m_shipInputHandler
    {
        get { return GetComponent<ShipInputHandler>(); }
    }

    /// <summary>
    /// the rigidbody component of the entire ship
    /// </summary>
    private Rigidbody m_shipRigidbody
    {
        get { return GetComponent<Rigidbody>(); }
    }

    [SerializeField] private Vector3 m_formattedShipInput;

    public float maxAccelleration;
    public float maxDecelleration;
    public const float maxSpeed = 100f;

    //public Vector3 targetVector
    //{
    //    //get
    //    {
    //        //return the current forward vector multiplied by the current value on a linear scale between no movement and max speed
    //    }
    //}


    [SerializeField] private ShipRCS m_shipRCS;


    

    



    //use fixed update to better deal with physics
    private void FixedUpdate()
    {
        //Debug.Log(targetVector);
    }


    //we need a decoder to unformat the code into a reasonable format
    private ShipInput InputDecoder(Vector3 input)
    {
        ShipInput newInput = new();
        newInput.JoystickValue = input; //the conversion is implicit
        newInput.ThrustValue = input.z;
        return newInput;
    }

    private void ApplyVectoredThrust(ShipInput input)
    {

    }



    //give a torque force to rotate
    private void ApplyTorqueForceAtLocation(Transform TorqueLocation)
    {

    }

    

    

}

public struct ShipInput
{
    public float ThrustValue;
    public Vector2 JoystickValue;
}
