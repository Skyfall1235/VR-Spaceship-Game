using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BC_Weapon
{
    [SerializeField] GameObject beamPrefab;
    GameObject beamInstance;
    LineRenderer lineRenderer;
    [SerializeField] float dps = 1;
    Collider lastHitCollider;
    float dpsTimer = 0;
    

    protected override void Awake()
    {
        base.Awake();
        beamInstance = Instantiate(beamPrefab, InstantiationPoint.transform.position, InstantiationPoint.transform.rotation);
        lineRenderer = beamInstance.GetComponentInChildren<LineRenderer>(); //just pray
        beamInstance.SetActive(false);
    }
    public override void OnFire()
    {
        
        beamInstance.transform.position = InstantiationPoint.transform.position;
        beamInstance.transform.rotation = InstantiationPoint.transform.rotation;
        

        Ray ray = new Ray(InstantiationPoint.transform.position, InstantiationPoint.transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider != lastHitCollider)
            {
                dpsTimer = 0;
                lastHitCollider = hit.collider;
            }
            Vector3 worldToLocalHit = new Vector3(0f, Vector3.Distance(InstantiationPoint.transform.position, hit.point), 0f);
            lineRenderer.SetPosition(1, worldToLocalHit);
        }
        else
        {
            lastHitCollider = null;
            dpsTimer = 0;
            lineRenderer.SetPosition(1, new Vector3(0f, 1000f, 0f));
        }
        if (lastHitCollider != null)
        {
            dpsTimer += dps * Time.fixedDeltaTime;
            if (dpsTimer >= 1)
            {
                IDamagable damagableComponent = lastHitCollider.GetComponent<IDamagable>();
                if (damagableComponent != null)
                {
                    Debug.Log("Damage Dealt");
                    damagableComponent.Damage(new DamageData(1, hit.point));
                }
                dpsTimer = 0;
            }
        }
    }
    public override void OnStartFire()
    {
        beamInstance.SetActive(true);
    }
    public override void OnStopFire()
    {
        dpsTimer = 0;
        beamInstance.SetActive(false);
    }
}
