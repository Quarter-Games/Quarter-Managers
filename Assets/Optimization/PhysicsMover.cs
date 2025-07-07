using UnityEngine;

public class PhysicsMover : MonoBehaviour
{
    public float force = 10f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * force);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 0.1f);
        }
    }
}
