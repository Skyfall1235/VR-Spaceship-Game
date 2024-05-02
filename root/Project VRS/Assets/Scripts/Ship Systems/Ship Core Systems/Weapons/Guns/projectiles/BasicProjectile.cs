using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BasicProjectile : MonoBehaviour
{
    public BC_Weapon gunThatFiredProjectile;
    [SerializeField] float timeToDestroyAfter;
    [SerializeField] ForceMode forceMode;
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    TrailRenderer m_trailRender;
    // Update is called once per frame

    private void Awake()
    {
        m_trailRender = GetComponent<TrailRenderer>();
    }
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToDestroyAfter);
        if(gunThatFiredProjectile != null)
        {
            //gunThatFiredProjectile.Pool.Release(gameObject);
        }
    }
    public void Setup(Vector3 startingPosition, Quaternion startingRotation)
    {
        rb.velocity = Vector3.zero;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        m_trailRender.Clear();
        rb.AddForce(transform.up * speed, forceMode);
        StartCoroutine(DestroyAfterTime());
    }
}

