using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ICoreModule;

public partial class AvionicsModule : BC_CoreModule
{
    #region Start up and Shut down logic
    protected override void PreStartUpLogic()
    {
        base.PreStartUpLogic();
        ShipRigidbody = transform.root.GetComponent<Rigidbody>();
        //setup for the inputs
        movementController.OnUpdateInputs.AddListener(ParseInput);
        movementController.OnUpdateInputs.AddListener(ParseInput);
    }

    protected override void PostStartUpLogic()
    {
        base.PostStartUpLogic();
        AreControlsResponsive = true;
    }

    protected override void PreShutDownLogic()
    {
        base.PreShutDownLogic();
        AreControlsResponsive = false;
        ShipRigidbody = null;
        //setup for the inputs
        movementController.OnUpdateInputs.RemoveListener(ParseInput);
        movementController.OnUpdateInputs.RemoveListener(ParseInput);
    }

    protected override void PostShutDownLogic()
    {
        base.PostShutDownLogic();
    }

    #endregion
}

