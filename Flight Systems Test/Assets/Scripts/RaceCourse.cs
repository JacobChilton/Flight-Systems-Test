using System.Collections.Generic;
using UnityEngine;

public class RaceCourse : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] CheckPoints;
    [SerializeField]private int currentCheckpointIndex = 0;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        List<GameObject> checkpointsList = new List<GameObject>();

        // Find all child objects with the "CheckPoints" tag
        foreach (Transform child in transform)
        {
            if (child.CompareTag("CheckPoints"))
            {
                checkpointsList.Add(child.gameObject);
            }
        }

        // Convert list to array
        CheckPoints = checkpointsList.ToArray();

        // Disable all checkpoints except the first one
        for (int i = 0; i < CheckPoints.Length; i++)
        {
            CheckPoints[i].SetActive(i == 0);
        }
    }

    // Function to be called by checkpoint scripts when a player reaches a checkpoint
    public void OnCheckpointReached(GameObject checkpoint)
    {
        // Check if the checkpoint is the correct one in sequence
        if (CheckPoints.Length == 0 || checkpoint != CheckPoints[currentCheckpointIndex]) return;

        // Disable the current checkpoint
        CheckPoints[currentCheckpointIndex].SetActive(false);

        // Move to the next checkpoint
        currentCheckpointIndex++;

        // Enable the next checkpoint if it exists
        if (currentCheckpointIndex < CheckPoints.Length)
        {
            CheckPoints[currentCheckpointIndex].SetActive(true);
            CheckPoints[currentCheckpointIndex + 1].SetActive(true);
        }
        else
        {
            Debug.Log("Race Finished");
            currentCheckpointIndex = 0;
            CheckPoints[currentCheckpointIndex].SetActive(true);
            CheckPoints[currentCheckpointIndex + 1].SetActive(true);
        }
        if (currentCheckpointIndex == 1)
        {
            gameManager.disableOtherRaces(gameObject);
        }
    }
}