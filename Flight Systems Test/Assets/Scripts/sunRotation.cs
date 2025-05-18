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

    private float sunTargetIntensity = 100000f;
    private float moonTargetIntensity = 80f;

    void Start()
    {
        currentDayLength = baseDayLengthInSeconds;
        isDay = transform.localEulerAngles.x < 180f;
        sunLight.intensity = isDay ? sunTargetIntensity : 0f;
        moonLight.intensity = isDay ? 0f : moonTargetIntensity;
    }

    private void Update()
    {
        float rotationSpeed = 360f / currentDayLength; // full rotation over a day
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        float angle = transform.localEulerAngles.x;

        if (angle > 180 && isDay)
        {
            Debug.Log("Night");
            isDay = false;
            currentDayLength = baseDayLengthInSeconds / 2f;
            //moonLight.enabled = true;
            //sunLight.enabled = false;
            sunTargetIntensity = 800f;
            moonTargetIntensity = 80f;
            isTransitioning = true;
            moonLight.enabled = true;
        }
        else if(angle < 180f && !isDay)
        {
            Debug.Log("Day");
            isDay = true;
            currentDayLength = baseDayLengthInSeconds;
            //sunLight.enabled = true;
            sunTargetIntensity = 100000;
            moonTargetIntensity = 0f;
            isTransitioning = true;
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
