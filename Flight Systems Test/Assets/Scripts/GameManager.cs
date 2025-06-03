using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] raceCourses, canyonWalls, canyonWallsOff;
    public bool raceStarted = false;
    public TextMeshProUGUI timeText;
    public GameObject canyonCollider1, canyonCollider2, settingsUI, settingsCamera, raceUI, missionUI;

    [Header("Settings UI")]
    public bool settingsEnabled = false;
    public CustomizationController customization;

    [Header("Clock")]
    public string currentTime;
    public int minutes = 0, hour = 0, seconds = 0;
    public bool realTime = false;
    public float clockSpeed = 1.0f;

    [Header("Race Times Display")]
    public TextMeshProUGUI countDownTimesText;
    public TextMeshProUGUI countUpTimesText;

    private float msecs = 0f;
    private PlayerControls controls;

    private Dictionary<string, float> countdownTimes = new Dictionary<string, float>();
    private Dictionary<string, float> countupTimes = new Dictionary<string, float>();

    void Awake()
    {
        controls = new PlayerControls();
        controls.Flight.ToggleSettings.performed += ctx => ToggleSettings();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        raceCourses = GameObject.FindGameObjectsWithTag("RaceCourse");
        canyonWalls = GameObject.FindGameObjectsWithTag("Canyon");

        List<GameObject> deactivatedWalls = new List<GameObject>();
        int wallOn = 0;

        for (int i = 0; i < canyonWalls.Length; i++)
        {
            if (wallOn != 0)
            {
                canyonWalls[i].SetActive(false);
                deactivatedWalls.Add(canyonWalls[i]);
            }
            wallOn = (wallOn + 1) % 4;
        }

        canyonWallsOff = deactivatedWalls.ToArray();
        canyonCollider1.SetActive(true);
        canyonCollider2.SetActive(true);
    }

    void Update()
    {
        if (!raceStarted) return;

        msecs += Time.deltaTime * clockSpeed;
        if (msecs >= 1.0f)
        {
            msecs -= 1.0f;
            seconds++;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
                if (minutes >= 60)
                {
                    minutes = 0;
                    hour = (hour + 1) % 24;
                }
            }
        }

        int displayMilliseconds = (int)(msecs * 1000);
        currentTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", hour, minutes, seconds, displayMilliseconds);
        if (timeText != null)
        {
            timeText.text = currentTime;
        }
    }

    public void disableOtherRaces(GameObject activeRaceCourse)
    {
        foreach (GameObject raceCourse in raceCourses)
        {
            if (raceCourse != activeRaceCourse)
            {
                raceCourse.SetActive(false);
            }
        }
        raceStarted = true;
        ResetGameClock();
        missionUI.SetActive(false);
        raceUI.SetActive(true);
    }

    public void enableOtherRaces(GameObject activeRaceCourse)
    {
        foreach (GameObject raceCourse in raceCourses)
        {
            if (raceCourse != activeRaceCourse)
            {
                raceCourse.SetActive(true);
            }
        }
        raceStarted = false;
        StartCoroutine(ReenableStartObjectAfterDelay(15f));
    }

    void ResetGameClock()
    {
        hour = 0;
        minutes = 0;
        seconds = 0;
        msecs = 0;
    }

    public void ToggleSettings()
    {
        if (settingsEnabled)
        {
            foreach (GameObject cam in customization.flightCameras)
                cam.SetActive(true);
            foreach (GameObject cam in customization.shopCameras)
                cam.SetActive(false);

            customization.flightUI.SetActive(true);
            Cursor.visible = false;
            Time.timeScale = 1;
            settingsUI.SetActive(false);
        }
        else
        {
            customization.ExitCustomization();

            foreach (GameObject cam in customization.flightCameras)
                cam.SetActive(false);
            foreach (GameObject cam in customization.shopCameras)
                cam.SetActive(false);

            customization.flightUI.SetActive(false);
            settingsCamera.SetActive(true);
            Cursor.visible = true;
            Time.timeScale = 0;
            settingsUI.SetActive(true);
        }

        settingsEnabled = !settingsEnabled;
    }

    public void UpdateCourseTime(RaceCourse course)
    {
        string courseName = course.name;

        if (course.countDirection)
        {
            countdownTimes[courseName] = course.finalTime;
        }
        else
        {
            countupTimes[courseName] = course.finalTime;
        }

        SaveBestTime(courseName, course.finalTime, course.countDirection);
        UpdateTimeDisplays();
    }

    void UpdateTimeDisplays()
    {
        countDownTimesText.text = "Countdown Courses:\n";
        foreach (var kvp in countdownTimes)
        {
            float best = LoadBestTime(kvp.Key, true);
            string bestDisplay = best >= 0f ? $"{best:F2}s" : "No record";
            countDownTimesText.text += $"{kvp.Key}:\nYour Best: {bestDisplay}\nTime: {kvp.Value:F2}s\n\n";
        }

        countUpTimesText.text = "Count-Up Courses:\n";
        foreach (var kvp in countupTimes)
        {
            float best = LoadBestTime(kvp.Key, false);
            string bestDisplay = best >= 0f ? $"{best:F2}s" : "No record";
            countUpTimesText.text += $"{kvp.Key}:\nYour Best: {bestDisplay}\nTime: {kvp.Value:F2}s\n\n";
        }
    }


    public void SaveBestTime(string courseName, float newTime, bool countDown)
    {
        string key = countDown ? $"BestLeft_{courseName}" : $"BestUp_{courseName}";

        if (PlayerPrefs.HasKey(key))
        {
            float savedTime = PlayerPrefs.GetFloat(key);

            if ((countDown && newTime > savedTime) || (!countDown && newTime < savedTime))
            {
                PlayerPrefs.SetFloat(key, newTime);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, newTime);
        }

        PlayerPrefs.Save();
    }

    public float LoadBestTime(string courseName, bool countDown)
    {
        string key = countDown ? $"BestLeft_{courseName}" : $"BestUp_{courseName}";
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : -1f;
    }

    private IEnumerator ReenableStartObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (missionUI != null)
        {
            missionUI.SetActive(true);
            raceUI.SetActive(false);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
