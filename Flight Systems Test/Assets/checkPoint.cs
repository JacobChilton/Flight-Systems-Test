using UnityEngine;

public class checkPoint : MonoBehaviour
{
    public RaceCourse raceCourse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raceCourse = Object.FindFirstObjectByType<RaceCourse>(); // Get reference to RaceCourse
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player is hitting the checkpoint
        {
            
            if (raceCourse != null)
            {
                raceCourse.OnCheckpointReached(gameObject);
            }
        }
        
    }
}
