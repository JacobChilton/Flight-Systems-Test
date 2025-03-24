using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // Import TextMeshPro

public class PlaneTest3 : MonoBehaviour
{
    [Header("Flight Settings")]
    public float maxThrottleForce = 5000f;
    public float currentThrottleForce;
    public float pitchSpeed = 50f;
    public float yawSpeed = 30f;
    public float rollSpeed = 80f;
    public float liftForce = 500f; // Increased for stronger effect
    public float airResistance = 0.99f;
    public bool propDead = false;
    public bool gearUp = true;

    [Header("UI Elements")]
    public TextMeshProUGUI speedText; // Assign TMP text in the inspector

    private float throttleInput;
    private float pitchInput;
    private float yawInput;
    private float rollInput;
    private bool isGrounded;
    private Rigidbody rb;
    private PlayerControls controls;
    private bool flapsDeployed = true;

    [Header("GameObjects")]
    public GameObject flapsUp;
    public GameObject flapsDown;
    public GameObject prop, LandGear;

    /*
     Controls:
     Throttle: W S
     Pitch: Q E
     Roll: A D
    */

    void Start() 
   {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 0.1f; // Unity 6 equivalent of drag
        rb.angularDamping = 0.5f;
        currentThrottleForce = 0.0f;
        LandingGear();
    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Flight.Flaps.performed += ctx => ToggleFlaps();
        controls.Flight.Respawn.performed += ctx => Respawn();
        controls.Flight.LandingGear.performed += ctx => LandingGear();
    }

    void Update()
    {
        // Get user input
        throttleInput = Mathf.Clamp(Input.GetAxis("Throttle"), -1f, 1f);
        pitchInput = Input.GetAxis("Vertical");
        yawInput = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");
         
        // Check if the plane is on the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f); // Increased from 1f

        // Update UI Speed Display
        UpdateSpeedUI();

        //Debug.Log($"Throttle: {throttleInput}, Roll: {rollInput}, Grounded: {isGrounded}");
    }

    void FixedUpdate()
    {
        //if (isGrounded)
        //{
        //    rb.linearVelocity = Vector3.zero; // FIXED: Using linearVelocity for Unity 6
        //    return;
        //}

        ApplyThrottle();
        ApplyLift();
        ApplyFlightControls();
    }

    void ApplyThrottle()
    {
        if (!propDead) //adjusts throttle if the propeller is not dead
        {
            currentThrottleForce += throttleInput;
        }
     
        currentThrottleForce = Mathf.Clamp(currentThrottleForce, 0, 100); // Prevent negative throttle

        Vector3 thrustForce = transform.forward * ((currentThrottleForce/100) * maxThrottleForce) * Time.deltaTime;
        rb.AddForce(thrustForce, ForceMode.Force);
    }

    void ApplyLift()
    {
        float speedFactor = Mathf.Clamp(rb.linearVelocity.magnitude, 0.5f, 15f); // FIXED: Using linearVelocity
        Vector3 lift = transform.up * liftForce * speedFactor * Time.deltaTime;
        rb.AddForce(lift, ForceMode.Force);

        // Apply air resistance only if moving fast
        if (rb.linearVelocity.magnitude > 1f)
        {
            rb.linearVelocity *= airResistance;
        }
    }

    void ApplyFlightControls() //assigns flight controls
    {
        rb.AddTorque(transform.right * pitchInput * pitchSpeed * Time.deltaTime, ForceMode.Force);
        rb.AddTorque(transform.up * yawInput * yawSpeed * Time.deltaTime, ForceMode.Force);
        rb.AddTorque(transform.forward * -rollInput * rollSpeed * Time.deltaTime, ForceMode.Force);
    }

    void UpdateSpeedUI()
    {
        if (speedText != null)
        {
            float airspeed = rb.linearVelocity.magnitude * 3.6f; // Convert from m/s to km/h
            speedText.text = $"Speed: {airspeed:F1} km/h\nVelocity: {rb.linearVelocity}\nThrottle:{Mathf.Ceil(currentThrottleForce)/*Rounds to whole number*/}\nAltitude:{(Mathf.Round(transform.position.y * 100)) / 100.0/*Rounds to 2 DP*/}\nFlaps: {flapsDeployed}";
        }
    }

    void OnEnable() { controls.Enable(); } //checks which input is pressed and directs to function
    void OnDisable() { controls.Disable(); }

    void ToggleFlaps() //sends which way to toggle flaps
    {
        flapsDeployed = !flapsDeployed;
        AdjustFlaps(flapsDeployed);
    }
    void AdjustFlaps(bool deployed)
    {
        Debug.Log("Flaps Adjusted");
        if (deployed) //decides which way to toggle flaps based on bool sent by input
        {
            if (!propDead) //only if prop is alive, causes issues with throttle amounts when it is supposed to be on 0 if dead.
            {
                maxThrottleForce -= 500;  // Increase drag to simulate air resistance
            }
            //rb.AddForce(Vector3.up * 300f, ForceMode.Force); // Extra lift
            liftForce += 30; //increases lift if flaps are deplyed
            flapsDown.SetActive(true);
            flapsUp.SetActive(false);
        }
        else
        {
            if (!propDead)
            {
                maxThrottleForce += 500;
            }
            liftForce -= 30; //decreases lift if flaps are raised
            flapsDown.SetActive(false);
            flapsUp.SetActive(true);
        }
    }
    void Respawn()
    {
        Debug.Log("Respawned");
        transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z); //raises plane by 20 on the Y axis
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); //redjusts plane angle
        prop.SetActive(true); //respawns propeller
        Debug.Log("prop back - respawned");
        propDead = false;
        maxThrottleForce = 3500; //resets throttle
        if (flapsDeployed) //adjusts throttle based on flaps and landing gear
        {
            maxThrottleForce -= 500;
        }
        if (!gearUp)
        {
            maxThrottleForce -= 100;
        }
    }
    public void deadProp() //destroys propeller and resets throttle to 0
    {
        maxThrottleForce = 0;
        prop.SetActive(false);
        Debug.Log("prop gone");
        propDead = true;
        currentThrottleForce = 0;
    }

    void LandingGear()
    {
        if (gearUp) //toggles landing gear and adjusts top speed to simulate match drag
        {
            LandGear.SetActive(true);
            gearUp = false;
            if (!propDead)
            {
                maxThrottleForce -= 100;
            }
        }
        else 
        { 
            LandGear.SetActive(false);
            gearUp = true;
            if (!propDead)
            {
                maxThrottleForce += 100;
            }
        }
    }
}
