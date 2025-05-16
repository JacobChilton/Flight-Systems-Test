using UnityEngine;
using System.Collections;

public class ClockOffline : MonoBehaviour
{
    //public bool realTime = false;
    [Header("Time")]
    public string currentTime;
    [Space]
    public int minutes = 0;
    public int hour = 0;
    public int seconds = 0;
    public bool realTime = false;
    public float clockSpeed = 30.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster - 30f = 2 min per hour

    //-- internal vars
    float msecs = 0;
    public float rotationSeconds;
    public float rotationMinutes;
    public float rotationHours;

    public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;

    void Start()
    {
        //roomManager = FindObjectOfType<RoomManagerOffline>(); //finds room manager script
    }

    void Update()
    {
        msecs += Time.deltaTime * clockSpeed; //calculate time and stores in variable
        if (msecs >= 1.0f)
        {
            msecs -= 1.0f;
            seconds++;

            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
                //Debug.Log(string.Format("{0:D2} - {1:D2} - {2:D2}", hour, minutes, seconds)); //updates log every game-speed minute
                if (minutes >= 60)
                {
                    minutes = 0;
                    hour++;
                    if (hour >= 24)
                        hour = 0;
                }
            }
        }


        rotationSeconds = (360.0f / 60.0f) * seconds;
        rotationMinutes = (360.0f / 60.0f) * minutes;
        rotationHours = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);
        currentTime = string.Format("{0:D2}:{1:D2}", hour, minutes); //updates current time variable every game-speed second
        ////-- calculate hand angles
        //float rotationSeconds = roomManager.rotationSeconds;
        //float rotationMinutes = roomManager.rotationMinutes;
        //float rotationHours = roomManager.rotationHours;

        //-- sets hand angles
        pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
        pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
        pointerHours.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);

    }
}
