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

        foreach (Transform child in transform) //Find all child objects with the "CheckPoints" tag
        {
            if (child.CompareTag("CheckPoints"))
            {
                checkpointsList.Add(child.gameObject);
            }
        }

        CheckPoints = checkpointsList.ToArray(); //Converts list to array

        for (int i = 0; i < CheckPoints.Length; i++) //Disable all checkpoints except the first one
        {
            CheckPoints[i].SetActive(i == 0);
        }
    }

    // Function called by checkpoint scripts when a player reaches a checkpoint
    public void OnCheckpointReached(GameObject checkpoint)
    { 
        if (CheckPoints.Length == 0 || checkpoint != CheckPoints[currentCheckpointIndex]) return; //Check if the checkpoint is the correct one in sequence(ensures no skipping)

        CheckPoints[currentCheckpointIndex].SetActive(false); //Disable the current checkpoint and move to next
        currentCheckpointIndex++; 

        if (currentCheckpointIndex < CheckPoints.Length)//Enable the next checkpoint if it exists
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
        if (currentCheckpointIndex == 1) //Disables the other courses if one is started
        {
            gameManager.disableOtherRaces(gameObject);
        }
    }
}