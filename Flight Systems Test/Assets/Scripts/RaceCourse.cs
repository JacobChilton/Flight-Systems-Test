using System.Collections.Generic;
using UnityEngine;

public class RaceCourse : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] CheckPoints;
    [SerializeField] private int currentCheckpointIndex = 0;

    public bool countDirection; // true = countdown, false = count up
    public float timeLimit;
    private float currentTime;
    private bool timerRunning = false;

    public float finalTime = 0f;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        List<GameObject> checkpointsList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("CheckPoints"))
            {
                checkpointsList.Add(child.gameObject);
            }
        }

        CheckPoints = checkpointsList.ToArray();

        for (int i = 0; i < CheckPoints.Length; i++)
        {
            CheckPoints[i].SetActive(i == 0);
        }

        currentCheckpointIndex = 0;
    }

    void Update()
    {
        if (!timerRunning) return;

        if (countDirection)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerRunning = false;
                Debug.Log("Race Failed: Time's up!");
                ResetCourse();
            }
        }
        else
        {
            currentTime += Time.deltaTime;
        }

        finalTime = currentTime;
    }

    public void OnCheckpointReached(GameObject checkpoint)
    {
        if (CheckPoints.Length == 0 || checkpoint != CheckPoints[currentCheckpointIndex]) return;

        CheckPoints[currentCheckpointIndex].SetActive(false);
        currentCheckpointIndex++;

        if (currentCheckpointIndex < CheckPoints.Length)
        {
            CheckPoints[currentCheckpointIndex].SetActive(true);
            if (currentCheckpointIndex < CheckPoints.Length - 1)
            {
                CheckPoints[currentCheckpointIndex + 1].SetActive(true);
            }
        }
        else
        {
            timerRunning = false;

            string msg = countDirection
                ? $"Finished Countdown with {finalTime:F2} seconds left"
                : $"Finished Count-Up in {finalTime:F2} seconds";
            Debug.Log(msg);

            gameManager.UpdateCourseTime(this);
            ResetCourse();
        }

        // If this is the start of the race (first checkpoint reached), start timer
        if (currentCheckpointIndex == 1)
        {
            currentTime = countDirection ? timeLimit : 0f;
            finalTime = currentTime;
            timerRunning = true;
            gameManager.disableOtherRaces(gameObject);
        }
    }

    void ResetCourse()
    {
        currentCheckpointIndex = 0;
        for (int i = 0; i < CheckPoints.Length; i++)
        {
            CheckPoints[i].SetActive(i == 0);
        }

        gameManager.enableOtherRaces(gameObject);
    }
}
