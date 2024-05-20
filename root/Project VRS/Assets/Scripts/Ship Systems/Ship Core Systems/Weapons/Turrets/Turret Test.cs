using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTest : MonoBehaviour
{
    [SerializeField]TargetHandler targetHandler;
    [SerializeField]TurretRotation turretRotation;
    private void Update()
    {
        if (targetHandler != null && turretRotation != null && targetHandler.RegisteredTargetsExcludingOverride.Count > 0)
        {
            turretRotation.TurnToLeadPosition(targetHandler.RegisteredTargetsExcludingOverride[0].TargetGameObject.transform.position);
        }
    }
}
