using System.Collections.Generic;
using TMPro;
//using UnityEditor.ShaderGraph;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] raceCourses, canyonWalls, canyonWallsOff;
    public bool raceStarted = false;
    public TextMeshProUGUI timeText;
    public GameObject canyonCollider1, canyonCollider2, settingsUI, settingsCamera;
    public int wallOn = 0;
    public bool settingsEnabled = false;
    [Header("Time")]
    public string currentTime;
    [Space]
    public int minutes = 0;
    public int hour = 0;
    public int seconds = 0;
    public bool realTime = false;
    public float clockSpeed = 1.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster - 30f = 2 min per hour
    private PlayerControls controls;
    public CustomizationController customization;

    //-- internal vars
    float msecs = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Flight.ToggleSettings.performed += ctx => ToggleSettings();
    }
    void Start()
    {
        raceCourses = GameObject.FindGameObjectsWithTag("RaceCourse");
        canyonWalls = GameObject.FindGameObjectsWithTag("Canyon");

        List<GameObject> deactivatedWalls = new List<GameObject>();

        for (int i = 0; i < canyonWalls.Length; i++)
        {
            if (wallOn != 0)
            {
                canyonWalls[i].SetActive(false);
                deactivatedWalls.Add(canyonWalls[i]);
            }
            wallOn++;
            if (wallOn == 4)
            {
                wallOn = 0;
            }
        }
        canyonWallsOff = deactivatedWalls.ToArray();
        canyonCollider1.SetActive(true);
        canyonCollider2.SetActive(true);
    }

    void OnEnable() { controls.Enable(); } //checks which input is pressed and directs to function
    void OnDisable() { controls.Disable(); }

    // Update is called once per frame
    void Update()
    {
        if(raceStarted){
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
            int displayMilliseconds = (int)(msecs * 1000);
            currentTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", hour, minutes, seconds, displayMilliseconds);
            timeText.text = currentTime;
}
    }
    public void disableOtherRaces(GameObject activeRaceCourse)
    {
        //for (int i = 0; i < raceCourses.Length; i++)
        //{
        //    if (raceCourses[i] == null)
        //    {
        //        raceCourses[i].SetActive(false);
        //    }
        //}

        foreach (GameObject raceCourse in raceCourses)
        {
            if (raceCourse != activeRaceCourse) // Disable all except the active one
            {
                raceCourse.SetActive(false);
            }
        }
        currentTime = "0";
        raceStarted = true;
    }
    public void enableOtherRaces(GameObject activeRaceCourse)
    {
        //for (int i = 0; i < raceCourses.Length; i++)
        //{
        //    if (raceCourses[i] == null)
        //    {
        //        raceCourses[i].SetActive(false);
        //    }
        //}

        foreach (GameObject raceCourse in raceCourses)
        {
            if (raceCourse != activeRaceCourse) // Disable all except the active one
            {
                raceCourse.SetActive(true);
            }
        }
        raceStarted = false;
    }
    public void ToggleSettings()
    {
        if (settingsEnabled)
        {
            for (int i = 0; i < customization.flightCameras.Length; i++)
            {
                customization.flightCameras[i].SetActive(true);
            }
            for (int i = 0; i < customization.shopCameras.Length; i++)
            {
                customization.shopCameras[i].SetActive(false);
            }
            customization.flightUI.SetActive(true);
            Cursor.visible = false;
            Time.timeScale = 1;
            settingsUI.SetActive(false);
            settingsEnabled = false;
        }
        else
        {
            customization.ExitCustomization();
            for (int i = 0; i < customization.flightCameras.Length; i++)
            {
                customization.flightCameras[i].SetActive(false);
            }
            for (int i = 0; i < customization.shopCameras.Length; i++)
            {
                customization.shopCameras[i].SetActive(false);
            }
            customization.flightUI.SetActive(false);
            settingsCamera.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0;
            settingsUI.SetActive(true);
            settingsEnabled = true;
        }
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
