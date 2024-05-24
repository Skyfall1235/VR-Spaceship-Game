using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Boid : MonoBehaviour
{
    public BoidManager Manager;
    List<GameObject> objectsToAffectMovement = new List<GameObject>();
    static int colliderSize = 10;
    private void Awake()
    {
        SphereCollider checkCollider = GetComponent<SphereCollider>();
        checkCollider.isTrigger = true;
        checkCollider.radius = colliderSize;
        foreach(RaycastHit hit in Physics.SphereCastAll(transform.position, colliderSize, transform.forward))
        {
            if(hit.collider.gameObject != this) 
            {
                objectsToAffectMovement.Add(hit.collider.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!objectsToAffectMovement.Contains(other.gameObject))
        {
            objectsToAffectMovement.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (objectsToAffectMovement.Contains(other.gameObject))
        {
            objectsToAffectMovement.Remove(other.gameObject);
        }
    }
    public Vector3 CenterOfMassRule()
    {
        if (Manager == null)
        {
            return Vector3.zero;
        }
        Vector3 calculatedCenterOfMass = Vector3.zero;
        foreach(Boid boid in Manager.Boids)
        {
            if(boid != this)
            {
                calculatedCenterOfMass += boid.transform.position;
            }
        }
        return (calculatedCenterOfMass / (Manager.Boids.Count - 1));
    }

    public Vector3 DistanceRule()
    {
        Vector3 vectorToReturn = Vector3.zero;
        foreach(GameObject affector in objectsToAffectMovement)
        {
            if (affector != gameObject)
            {
                if (Vector3.Distance(transform.position, affector.transform.position) < 2)
                {
                    vectorToReturn = vectorToReturn - (affector.transform.position - transform.position);
                }
            }
        }
        return vectorToReturn;
    }
}
