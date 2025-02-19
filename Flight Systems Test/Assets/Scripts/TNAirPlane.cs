using UnityEngine;

public class TNAirPlane : MonoBehaviour
{
    public float thrust = 10f;
    public float topSpeed = 20f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

   void Update()
    {
        if(rb.angularVelocity.magnitude < topSpeed)
        {
            rb.AddRelativeForce(new Vector3(0, 0, thrust));
        }
    }
}
