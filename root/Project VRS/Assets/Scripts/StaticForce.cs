using UnityEngine;

public class StaticForce : MonoBehaviour
{
    public ForceMode forceMode = ForceMode.Force;
    public float speed;
    public Vector3 Direction;
    public Rigidbody rb;
    // Update is called once per frame
    void Update()
    {
        Vector3 dirNormalized = Vector3.Normalize(Direction);
        rb.AddRelativeForce(dirNormalized * speed, forceMode);
    }

}
