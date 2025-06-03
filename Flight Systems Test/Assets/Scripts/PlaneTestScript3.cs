using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro

public class PlaneTest3 : MonoBehaviour
{
    [Header("Flight Settings")]
    public float defaultMaxThrottle = 3500f;
    public float maxThrottleForce = 3500f;
    public float currentThrottleForce;
    public float pitchSpeed = 50f;
    public float yawSpeed = 30f;
    public float rollSpeed = 80f;
    public float liftForce = 500f; // Increased for stronger effect
    public float airResistance = 0.99f;
    public bool propDead = false;
    public bool gearUp = true;
    public bool controlsActive = true;

    private Vector2 mouseDelta;
    public float mouseYawSensitivity = 0.1f; // Sensitivity multiplier
    public float mousePitchSensitivity = 0.1f;

    [Header("Propeller Settings")]
    public float maxPropellerSpeed = 2000f; // Degrees per second
    public float propellerSpinUpRate = 5f;  // Smooth spin-up
    private float currentPropellerSpeed;
    public AudioSource audioSource;

    [Header("UI Elements")]
    public TextMeshProUGUI speedText; // Assign TMP text in the inspector

    private float throttleInput;
    private float pitchInput;
    private float yawInput;
    private float rollInput;
    [SerializeField] private bool isGrounded, groundThrottle;
    private Rigidbody rb;
    private PlayerControls controls;
    private bool flapsDeployed = true;
    public float airspeed;
    private bool isFreeLook = false;
    [SerializeField] private RectTransform mouseIndicatorUI;
    private Vector2 uiTargetPos;
    private Vector2 smoothedMouseIndicatorPos;
    public float mouseIndicatorSmoothSpeed = 5f;
    [SerializeField] private CanvasGroup mouseIndicatorCanvasGroup;
    public float fadeSpeed = 1f;

    [Header("GameObjects")]
    public GameObject flapsUp;
    public GameObject flapsDown;
    public GameObject prop;
    [SerializeField] private Camera followCam;
    [SerializeField] private Camera orbitCam;

    [Header("Landing Gear Transforms")]
    public Transform[] landingGearParts; // All visual gear parts
    public Transform[] landingGearTargetUp;
    public Transform[] landingGearTargetDown;

    public float gearMoveSpeed = 2f; // Higher = faster movement
    private bool gearAnimating = false;
    private bool movingGearUp = false;

    [Header("Mission Objectives")]
    public List<Objective> objectives;
    private int currentObjectiveIndex = 0;

    [Header("UI")]
    public TextMeshProUGUI currentObjectiveText;
    public TextMeshProUGUI objectiveText;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateObjectiveUI();
        foreach (Objective obj in objectives)
        {
            if (obj == null) continue;

            Debug.Log("Objective: " + obj.objectiveName);
        }

    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Flight.Flaps.performed += ctx => ToggleFlaps();
        controls.Flight.Respawn.performed += ctx => Respawn();
        controls.Flight.LandingGear.performed += ctx => LandingGear();
        controls.Flight.FreeLookToggle.performed += ctx => ToggleFreeLook(true);
        controls.Flight.FreeLookToggle.canceled += ctx => ToggleFreeLook(false);
        controls.Flight.MouseYaw.performed += ctx => mouseDelta = ctx.ReadValue<Vector2>();
        controls.Flight.MouseYaw.canceled += ctx => mouseDelta = Vector2.zero;
    }

    void Update()
    {
        // Get user input
        throttleInput = Mathf.Clamp(Input.GetAxis("Throttle"), -1f, 1f);
        //pitchInput = Input.GetAxis("Vertical");
        //yawInput = Input.GetAxis("Horizontal");
        rollInput = Input.GetAxis("Roll");

        Vector2 clampedMouseDelta = new Vector2(Mathf.Clamp(mouseDelta.x, -20f, 20f), Mathf.Clamp(mouseDelta.y, -20f, 20f));

        float fadeTarget = (clampedMouseDelta.magnitude > 0.1f && !isFreeLook) ? 1f : 0f;
        mouseIndicatorCanvasGroup.alpha = Mathf.Lerp(mouseIndicatorCanvasGroup.alpha, fadeTarget, Time.deltaTime * fadeSpeed);

        if (mouseIndicatorUI != null && !isFreeLook)
        {
            float maxVisualRange = 500f; // UI range on screen for visualized movement
            Vector2 visualDelta = new Vector2(
                Mathf.Clamp(mouseDelta.x * maxVisualRange / 20f, -maxVisualRange, maxVisualRange),
                Mathf.Clamp(mouseDelta.y * maxVisualRange / 20f, -maxVisualRange, maxVisualRange)
            );

            // Center of screen + offset
            uiTargetPos = Vector2.Lerp(uiTargetPos, visualDelta, Time.deltaTime * 10f);
            smoothedMouseIndicatorPos = Vector2.Lerp(smoothedMouseIndicatorPos, visualDelta, Time.deltaTime * mouseIndicatorSmoothSpeed);
            mouseIndicatorUI.anchoredPosition = smoothedMouseIndicatorPos;
        }
        else if (mouseIndicatorUI != null)
        {
            // Hide or reset in Free Look mode
            mouseIndicatorUI.anchoredPosition = new Vector2(0, -229.5f);
        }
        if (!isFreeLook)
        {
            float keyboardPitch = Input.GetAxis("Vertical"); // Optional: Keep for keyboard support
            float keyboardYaw = Input.GetAxis("Horizontal");
            // Invert mouse Y for natural aircraft control (push down = nose down)

            //pitchInput = keyboardPitch - mouseDelta.y * mousePitchSensitivity;
            //yawInput = keyboardYaw + mouseDelta.x * mouseYawSensitivity;
            pitchInput = keyboardPitch - clampedMouseDelta.y * mousePitchSensitivity;
            yawInput = keyboardYaw + clampedMouseDelta.x * mouseYawSensitivity;
        }
        else
        {
            pitchInput = Input.GetAxis("Vertical"); // Optional: allow keyboard input during freelook
            yawInput = Input.GetAxis("Horizontal");
        }

        // Check if the plane is on the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f); // Increased from 1f

        // Update UI Speed Display
        UpdateSpeedUI();

        mouseDelta = Vector2.zero; // Clear delta each frame

        //Debug.Log($"Throttle: {throttleInput}, Roll: {rollInput}, Grounded: {isGrounded}");

        // Calculate desired propeller speed
        float targetSpeed = (currentThrottleForce / 100f) * maxPropellerSpeed;

        // Smooth the spin-up/down of the propeller
        currentPropellerSpeed = Mathf.Lerp(currentPropellerSpeed, targetSpeed, Time.deltaTime * propellerSpinUpRate);

        // Rotate the propeller
        if (prop != null && prop.activeSelf)
        {
            prop.transform.Rotate(Vector3.forward, currentPropellerSpeed * Time.deltaTime, Space.Self);
        }

        if (gearAnimating && landingGearParts.Length > 0)
        {
            bool allLerped = true;

            for (int i = 0; i < landingGearParts.Length; i++)
            {
                Transform part = landingGearParts[i];
                Transform target = gearUp ? landingGearTargetUp[i] : landingGearTargetDown[i];

                if (part == null || target == null) continue;

                part.position = Vector3.Lerp(part.position, target.position, Time.deltaTime * gearMoveSpeed);
                part.rotation = Quaternion.Lerp(part.rotation, target.rotation, Time.deltaTime * gearMoveSpeed);
                part.localScale = Vector3.Lerp(part.localScale, target.localScale, Time.deltaTime * gearMoveSpeed); //  THIS LINE

                if (Vector3.Distance(part.position, target.position) > 0.01f ||
                    Quaternion.Angle(part.rotation, target.rotation) > 0.5f ||
                    Vector3.Distance(part.localScale, target.localScale) > 0.01f)
                {
                    allLerped = false;
                }
            }

            if (allLerped)
            {
                gearAnimating = false;

                // Snap to final values for precision
                for (int i = 0; i < landingGearParts.Length; i++)
                {
                    Transform part = landingGearParts[i];
                    Transform target = gearUp ? landingGearTargetUp[i] : landingGearTargetDown[i];
                    if (part == null || target == null) continue;

                    part.position = target.position;
                    part.rotation = target.rotation;
                    part.localScale = target.localScale;
                }
            }
        }

        if (isGrounded && !groundThrottle && airspeed < 100f)
        {
            maxThrottleForce -= 1200f;
            groundThrottle = !groundThrottle;
        }
        else if (!isGrounded && groundThrottle)
        {
            if (!propDead)
            {
                maxThrottleForce += 1200f;
            }
            groundThrottle = !groundThrottle;
        }
        if (isGrounded && airspeed < 1f && mouseIndicatorCanvasGroup.alpha < 0.02f)
        {
            ToggleFreeLook(true);
        }
        else if (isGrounded && airspeed > 1)
        {
            ToggleFreeLook(false);
        }

        if (!controlsActive)
        {
            maxThrottleForce = 0f;
        }

    }

    void FixedUpdate()
    {
        ApplyThrottle();
        if (controlsActive)
        {         
            ApplyLift();
            ApplyFlightControls();
        }
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
            airspeed = rb.linearVelocity.magnitude * 3.6f; // Convert from m/s to km/h
            speedText.text = $"Speed: {airspeed:F1} km/h\nThrottle:{Mathf.Ceil(currentThrottleForce)/*Rounds to whole number*/}\nAltitude:{(Mathf.Round(transform.position.y * 100)) / 100.0/*Rounds to 2 DP*/}\nFlaps: {flapsDeployed}";
            float throttlePercent = currentThrottleForce / 100f;

            // Volume: 0 when throttle is 0, up to ~0.8 max
            audioSource.volume = Mathf.Lerp(0f, 0.1f, throttlePercent);

            // Pitch: base 1.0, increase slightly (up to ~1.2)
            audioSource.pitch = Mathf.Lerp(0.5f, 1.1f, throttlePercent);

            // Optional: Stop audio if throttle is 0 to save resources
            if (currentThrottleForce <= 0.01f)
            {
                if (audioSource.isPlaying) audioSource.Stop();
            }
            else
            {
                if (!audioSource.isPlaying) audioSource.Play();
            }

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
        maxThrottleForce = defaultMaxThrottle; //resets throttle
        if (groundThrottle)
        {
            maxThrottleForce -= 1200;
        }
        Debug.Log("Respawned");
        transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z); //raises plane by 20 on the Y axis
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0); //redjusts plane angle
        prop.SetActive(true); //respawns propeller
        Debug.Log("prop back - respawned");
        propDead = false;
        if (flapsDeployed) //adjusts throttle based on flaps and landing gear
        {
            maxThrottleForce -= 500;
        }
        if (!gearUp)
        {
            maxThrottleForce -= 100;
        }
        ToggleFreeLook(false);
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
        //if (gearAnimating) return; // Prevent spamming while animating

        //movingGearUp = !gearUp; // If gear is currently up, this will bring it down
        gearAnimating = true;
        
        if (gearUp) //toggles landing gear and adjusts top speed to simulate match drag
        {
            gearUp = !gearUp;
            if (!propDead)
            {
                maxThrottleForce -= 100;
            }
        }
        else 
        {
            gearUp = !gearUp;
            if (!propDead)
            {
                maxThrottleForce += 100;
            }
        }
    }
    private void ToggleFreeLook(bool enable)
    {
        followCam.enabled = !enable;
        orbitCam.enabled = enable;
        isFreeLook = enable;
        if (enable)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void UpdateObjectiveUI()
    {
        if (objectiveText == null || objectives.Count == 0) return;

        string display = "<b>Mission Objectives:</b>\n";
        string display2 = "<b>Current Objective:</b>\n";
        for (int i = 0; i < objectives.Count; i++)
        {
            if (i == currentObjectiveIndex)
                display2 += $"<color=yellow>? {objectives[i].objectiveName}</color>\n";
            else if (objectives[i].isCompleted)
                display += $"<color=green>? {objectives[i].objectiveName}</color>\n";
            else
                display += $"• {objectives[i].objectiveName}\n";
        }
        currentObjectiveText.text = display2;
        objectiveText.text = display;
    }
    public void CheckObjectiveCompletion(string reachedOutpost)
    {
        if (currentObjectiveIndex >= objectives.Count) return;

        Objective current = objectives[currentObjectiveIndex];
        if (current.targetLocation.name == reachedOutpost && !current.isCompleted)
        {
            current.isCompleted = true;
            currentObjectiveIndex++;

            Debug.Log($"Objective '{current.objectiveName}' completed!");
            UpdateObjectiveUI();
        }
    }

}
[System.Serializable]
public class Objective
{
    public string objectiveName; // e.g., "Deliver to Outpost Bravo"
    public Transform targetLocation; // Location to reach (e.g., runway collider or marker)
    public bool isCompleted = false;
}