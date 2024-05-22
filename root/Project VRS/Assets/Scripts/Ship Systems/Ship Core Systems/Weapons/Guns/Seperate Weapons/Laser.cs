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
        beamInstance = Instantiate(beamPrefab, InstantiationPoint.transform.position, InstantiationPoint.transform.rotation);
        beamInstance.SetActive(false);
    }
    public override void OnFire()
    {
        beamInstance.transform.position = InstantiationPoint.transform.position;
        beamInstance.transform.rotation = InstantiationPoint.transform.rotation;;
    }
    public override void OnStartFire()
    {
        beamInstance.SetActive(true);
    }
    public override void OnStopFire()
    {
        beamInstance.SetActive(false);
    }
}
