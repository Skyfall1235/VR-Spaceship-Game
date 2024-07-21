using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAvionicsModule : AvionicsModule
{
    [Header("Player Avionics Data")]
    //requeired to store the casted input handler
    [SerializeField] PlayerShipInputHandler m_playerShipInputHandler;

    #region Update events for input on controllers


    #endregion

    public override void Awake()
    {
        base.Awake();
        ShipRigidbody = transform.root.GetComponent<Rigidbody>();
        try
        {
            m_playerShipInputHandler = (PlayerShipInputHandler)shipInputHandler;
        }
        catch
        {
            Debug.Log("input could not be converted");
            throw new System.Exception();
        }
    }

    private void FixedUpdate()
    {
        //this should probably be more optimised, but i will just come back to it later
        RetrieveRigidbodyInfo();
        CalculateIntertialForce();
    }

    #region Data aggregation

    protected override void PreStartUpLogic()
    {
        base.PreStartUpLogic();
        //setup for the inputs
        m_playerShipInputHandler.InputRecieval.AddListener(ParseInput);

    }
    protected override void PreShutDownLogic()
    {
        base.PreShutDownLogic();
        //removal of input access
        m_playerShipInputHandler.InputRecieval.RemoveListener(ParseInput);
    }

    //parse input is only called if the ship input struct return is reached, so if inputs arent reached then
    void ParseInput()
    {
        
    }

    #endregion
}

