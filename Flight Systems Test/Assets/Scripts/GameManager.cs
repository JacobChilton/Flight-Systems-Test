using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] raceCourses;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raceCourses = GameObject.FindGameObjectsWithTag("RaceCourse");
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
