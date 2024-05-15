using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BC_Weapon
{
    [SerializeField] GameObject beamPrefab;
    GameObject beamInstance;
    protected override void Awake()
    {
        base.Awake();
        beamInstance = Instantiate(beamPrefab, InstantiationPoint.transform, false);
        beamInstance.SetActive(false);
    }
    public override void OnFire()
    {
        
    }
    public override void OnStartFire()
    {
        beamInstance.SetActive(true);
    }
    public override void OnEndFire()
    {
        beamInstance.SetActive(false);
    }
}
