using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidRB : MonoBehaviour
{
    int distanceThreshold = 10;
    static List<BoidRB> ActiveBoids = new List<BoidRB>();
    Rigidbody rb;
    private void Awake()
    {
        if (!ActiveBoids.Contains(this)) 
        {
            ActiveBoids.Add(this); 
        }
        rb = GetComponent<Rigidbody>();
    }
    private void OnDestroy()
    {
        if (ActiveBoids.Contains(this))
        {
            ActiveBoids.Remove(this);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(DistanceRule() + CenterOfMassRule() + VelocityChangeRule());
        Debug.DrawRay(transform.position, rb.velocity);
    }

    private Vector3 CenterOfMassRule()
    {
        Vector3 headingChange = Vector3.zero;
        foreach(BoidRB boid in ActiveBoids)
        {
            if(boid != this)
            {
                headingChange += boid.transform.position;
            }
        }
        headingChange = headingChange / (ActiveBoids.Count - 1);
        return ((headingChange - transform.position) /100);
    }
    private Vector3 DistanceRule()
    {
        Vector3 headingChange = Vector3.zero;
        foreach(BoidRB boid in ActiveBoids)
        {
            if(boid != this)
            {
                if((boid.transform.position - transform.position).Abs().magnitude < distanceThreshold)
                {
                    headingChange -= (boid.transform.position - transform.position);
                }
            }
        }
        return headingChange;
    }

    private Vector3 VelocityChangeRule()
    {
        Vector3 headingChange = Vector3.zero;
        foreach(BoidRB boid in ActiveBoids)
        {
            if(boid != this)
            {
                headingChange += boid.rb.velocity;
            }
        }
        headingChange = headingChange / (ActiveBoids.Count - 1);
        return (headingChange / 8);
    }
}
