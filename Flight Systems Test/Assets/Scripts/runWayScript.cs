using UnityEngine;

public class runwayScript : MonoBehaviour
{
    private bool playerInZone = false;
    public GameObject promptUI; // Assign your UI element in the inspector
    public PlaneTest3 plane;
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            float airspeed = plane.airspeed;
            if (airspeed < 1f)
            {
                gameManager.GetComponent<CustomizationController>().EnterCustomization();
                promptUI.SetActive(false);
            }
        }
    }
}