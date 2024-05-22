using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public List<Boid> Boids { get; private set; }

    private void Awake()
    {
        Boids = FindObjectsByType<Boid>(FindObjectsSortMode.None).ToList();
        if(Boids.Count > 0)
        {
            foreach (Boid boid in Boids)
            {
                boid.Manager = this;
            }
        }
    }
    private void FixedUpdate()
    {
        foreach(Boid boid in Boids)
        {
            boid.GetComponent<Rigidbody>().AddForce(boid.CenterOfMassRule() + boid.DistanceRule());
            Debug.DrawRay(boid.gameObject.transform.position, boid.CenterOfMassRule() + boid.DistanceRule());
        }
    }
}
