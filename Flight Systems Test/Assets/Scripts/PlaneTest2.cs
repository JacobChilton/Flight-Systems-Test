using UnityEngine;
using TMPro; // For UI display (optional, can be removed if not needed)

public class PlaneTest2 : MonoBehaviour
{
    [Header("Flight Settings")]
    public float throttlePower = 50f;
    public float maxSpeed = 150f;
    public float minSpeed = 20f;
    public float lift = 100f;
    public float pitchSpeed = 3f;
    public float yawSpeed = 2f;
    public float rollSpeed = 3f;
    public float turnResponsiveness = 2f;
    public float maxThrottleForce = 5000f;
    public float currentThrottleForce;

    [Header("UI Elements")]
    public TextMeshProUGUI airspeedText; // Optional, for HUD display

    private Rigidbody rb;
    private float throttleInput;
    private float rollInput;
    private Vector2 screenCenter;
    private Vector2 mouseOffset;
    private Vector2 mousePosition;
    private float airspeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; // Enable gravity for realistic flight
        rb.mass = 200f; // Adjust as needed for better flight feel
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        currentThrottleForce = 0.0f;
    }

    void Update()
    {
        
        throttleInput = Mathf.Clamp(Input.GetAxis("Throttle"), -1f, 1f); //Capture throttle and roll inputs
        rollInput = (Input.GetKey(KeyCode.D) ? -1 : 0) + (Input.GetKey(KeyCode.A) ? 1 : 0);
       
        mousePosition = Input.mousePosition; //Get mouse position and calculate offset from screen center
        mouseOffset = (mousePosition - screenCenter) / screenCenter;
        
        airspeed = rb.linearVelocity.magnitude * 3.6f; //Calculate speed from Rigidbody velocity magnitude and convert from m/s to km/h
        if (airspeedText) airspeedText.text = "Speed: " + Mathf.Round(airspeed) + " km/h";
    }

    void FixedUpdate()
    {
        ApplyThrottle(); //Applies throttle force every frame

        float pitch = -mouseOffset.y * pitchSpeed;
        float yaw = mouseOffset.x * yawSpeed;
        float roll = rollInput * rollSpeed;

        rb.AddTorque(transform.right * pitch * turnResponsiveness, ForceMode.Acceleration);
        rb.AddTorque(transform.up * yaw * turnResponsiveness, ForceMode.Acceleration);
        rb.AddTorque(transform.forward * roll * turnResponsiveness, ForceMode.Acceleration);
    }
    void ApplyThrottle()
    {
        currentThrottleForce += throttleInput;
        currentThrottleForce = Mathf.Clamp(currentThrottleForce, 0, 100); // Prevent negative throttle

        Vector3 thrustForce = transform.forward * ((currentThrottleForce / 100) * maxThrottleForce) * Time.deltaTime;
        rb.AddForce(thrustForce, ForceMode.Force);
    }
}



// Apply forward force for throttle
//float currentSpeed = rb.linearVelocity.magnitude;
//if ((currentSpeed < maxSpeed && throttleInput > 0) || (currentSpeed > minSpeed && throttleInput < 0))
//{
//    rb.AddForce(transform.forward * throttleInput * throttlePower, ForceMode.Acceleration);
//}

//// Apply aerodynamic lift to keep the plane in the air
//rb.AddForce(Vector3.up * lift * Mathf.Clamp(currentSpeed / maxSpeed, 0, 1), ForceMode.Force);

// Apply rotation forces based on mouse movement