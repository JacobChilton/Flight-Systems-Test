using UnityEngine;

public class TNAirPlane : MonoBehaviour
{
    public float thrust = 10f;
    public float topSpeed = 20f;
    public float liftForce = 5f;
    public float yawTorque = 2f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //Finds the rigidbody of the parent object
    }

   void Update()
    {
        if(rb.angularVelocity.magnitude < topSpeed)
        {
            rb.AddRelativeForce(new Vector3(0, 0, thrust)); //Adds the entered thrust force at a consistent rate
        }
        rb.AddForce(Vector3.up * liftForce); //Applies the lift force

        float yawInput = 0f;
        if (Input.GetKey(KeyCode.A)) yawInput = -1f; //Gets yaw controls
        if (Input.GetKey(KeyCode.D)) yawInput = 1f;


        rb.AddRelativeTorque(Vector3.up * yawInput * yawTorque); //Applies yaw forces
    }
}
