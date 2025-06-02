using System;
using JetBrains.Annotations;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float baseDayLengthInSeconds = 1200f; // 20-minute day
    [SerializeField]
    private float currentDayLength;
    [SerializeField]
    private bool isDay = false;
    private bool isTransitioning = false;
    public Light moonLight, sunLight;

    [Range(0, 10000)] public float sunTransitionSpeed = 1f;
    [Range(0, 1000)] public float moonTransitionSpeed = 0.2f;

    private float sunTargetIntensity = 50000f;
    private float moonTargetIntensity = 12.5f;

    [Header("Time of Day")]
    public int hour;
    public int minute;
    public string timeString;
    private float timeOfDayInSeconds = 0f; // New timer variable
    private float logicalSunAngle = 0f;



    void Start()
    {
        currentDayLength = baseDayLengthInSeconds;
        //isDay = transform.localEulerAngles.x < 180f;
        //sunLight.intensity = isDay ? sunTargetIntensity : 0f;
        //moonLight.intensity = isDay ? 0f : moonTargetIntensity;


        float angle = transform.localEulerAngles.x % 360f;
        logicalSunAngle = angle;

        // Initialize time from angle using same logic as Update
        float daylightHours = 16f;
        float nightHours = 8f;
        float currentHour;

        if (angle <= 180f)
        {
            currentHour = 4f + (angle / 180f) * daylightHours;
        }
        else
        {
            currentHour = 20f + ((angle - 180f) / 180f) * nightHours;
        }

        if (currentHour >= 24f) currentHour -= 24f;

        timeOfDayInSeconds = currentHour * 3600f;

        hour = Mathf.FloorToInt(currentHour);
        minute = Mathf.FloorToInt((currentHour - hour) * 60f);
        timeString = string.Format("{0:00}:{1:00}", hour, minute);

        isDay = angle < 180f;
        sunLight.intensity = isDay ? sunTargetIntensity : 0f;
        moonLight.intensity = isDay ? 0f : moonTargetIntensity;
    }

    private void Update()
    {
        // --- Track the rotation ---
        float rotationSpeed = 360f / currentDayLength;
        float step = rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.right, step);
        logicalSunAngle = (logicalSunAngle + step) % 360f;

        // --- Time from Angle ---
        float daylightHours = 16f;
        float nightHours = 8f;
        float angle = logicalSunAngle % 360f;
        float currentHour;

        if (angle <= 180f)
        {
            // Day: from 0° (04:00) to 180° (20:00)
            currentHour = 4f + (angle / 180f) * daylightHours;
        }
        else
        {
            // Night: from 180° (20:00) to 360° (04:00)
            currentHour = 20f + ((angle - 180f) / 180f) * nightHours;
        }

        // Wrap around if over 24
        if (currentHour >= 24f) currentHour -= 24f;

        hour = Mathf.FloorToInt(currentHour);
        minute = Mathf.FloorToInt((currentHour - hour) * 60f);
        timeString = string.Format("{0:00}:{1:00}", hour, minute);
        //Debug.Log("Time: " + timeString);

        if (angle > 180 && isDay)
        {
            Debug.Log("Night");
            isDay = false;
            currentDayLength = baseDayLengthInSeconds / 2f;
            //moonLight.enabled = true;
            //sunLight.enabled = false;
            sunTargetIntensity = 100f;
            moonTargetIntensity = 12.5f;
            isTransitioning = true;
            moonLight.enabled = true;
            moonTransitionSpeed = 0.2f;
        }
        else if(angle < 180f && !isDay)
        {
            Debug.Log("Day");
            isDay = true;
            currentDayLength = baseDayLengthInSeconds;
            //sunLight.enabled = true;
            sunTargetIntensity = 50000;
            moonTargetIntensity = 1f;
            isTransitioning = true;
            moonTransitionSpeed = 10f;
        }
        else if(angle > -10f  && isDay)
        {
            Debug.Log("sun On");
            sunLight.enabled = true;
        }

        if (isTransitioning)
        {
            fadeLight(sunTargetIntensity, moonTargetIntensity);
        }


    }
    void fadeLight(float sunLightVal, float moonLightVal)
    {
        sunLight.intensity = Mathf.MoveTowards(sunLight.intensity, sunLightVal, Time.deltaTime * sunTransitionSpeed);
        moonLight.intensity = Mathf.MoveTowards(moonLight.intensity, moonLightVal, Time.deltaTime * moonTransitionSpeed);

        if (Mathf.Approximately(sunLight.intensity, sunLightVal) && Mathf.Approximately(moonLight.intensity, moonLightVal))
        {
            isTransitioning = false;
            if (isDay)
            {
                moonLight.enabled = false;
            }
            else
            {
                sunLight.enabled = false;
            }
        }
        
    }
}
