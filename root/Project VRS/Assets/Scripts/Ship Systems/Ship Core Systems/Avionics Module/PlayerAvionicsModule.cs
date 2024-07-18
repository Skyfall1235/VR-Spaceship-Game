using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAvionicsModule : AvionicsModule
{
    [SerializeField] PlayerShipInputHandler m_playerShipInputHandler;

    #region action values

    [SerializeField] ActionValues? m_primaryInput;
    public ActionValues? PrimaryInput
    {
        get => m_primaryInput; 
        set
        {
            m_primaryInput = value;
            PrimaryInputUpdate.Invoke(m_primaryInput);
        }
    }
    [SerializeField] ActionValues? m_secondaryInput;
    public ActionValues? SecondaryInput
    {
        get => m_secondaryInput;
        set
        {
            m_secondaryInput = value;
            PrimaryInputUpdate.Invoke(m_secondaryInput);
        }
    }

    #endregion

    #region Update events for input on controllers

    public UnityEvent<ActionValues?> PrimaryInputUpdate = new UnityEvent<ActionValues?>();
    public UnityEvent<ActionValues?> SecondaryInputsUpdate = new UnityEvent<ActionValues?>();

    #endregion

    public override void Awake()
    {
        base.Awake();
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
        RetrievePosition();
        //RetrieveRigidbodyInfo();
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

    void ParseInput()
    {
        //check to confirm controls are allowed to send inputs
        if(AreControlsResponsive)
        {
            PrimaryInput = m_playerShipInputHandler.PrimaryValuesProperties.RetrieveActionValues();
            SecondaryInput = m_playerShipInputHandler.SecondaryValuesProperties.RetrieveActionValues();
        }
    }

    void RetrievePosition()
    {
        PositionInSpace = transform.position;
    }

    #endregion
}

public interface IControllerActionValues
{
    public void SetEvents(UnityEvent<ActionValues?> primary, UnityEvent<ActionValues?> secondary);
    public void RemoveEvents(UnityEvent<ActionValues?> primary, UnityEvent<ActionValues?> secondary);

    public void PrimaryUpdate(ActionValues? primary);

    public void SecondaryUpdate(ActionValues? secondary);
}

