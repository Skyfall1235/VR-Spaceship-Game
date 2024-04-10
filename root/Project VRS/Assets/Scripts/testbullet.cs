using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testbullet : MonoBehaviour
{
    public ForceMode forceMode;
    public float speed;
    public Rigidbody rb;
    // Update is called once per frame
    void Start()
    {
        rb.AddForce(transform.up * speed, forceMode);
    }
}
