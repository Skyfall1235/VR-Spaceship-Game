using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidRB : MonoBehaviour
{
    int distanceThreshold = 2;
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
        rb.velocity = rb.velocity + CenterOfMassRule() + DistanceRule();
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
        return ((headingChange - transform.position) /100) * Time.fixedDeltaTime;
    }
    private Vector3 DistanceRule()
    {
        Vector3 headingChange = Vector3.zero;
        foreach(BoidRB boid in ActiveBoids)
        {
            if(boid != this)
            {
                if(Vector3.Distance(transform.position, boid.transform.position) < distanceThreshold)
                {
                    headingChange = headingChange - (boid.transform.position - transform.position);
                }
            }
        }
        return headingChange * Time.fixedDeltaTime ;
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
        return (headingChange / 8) * Time.fixedDeltaTime;
    }
}
