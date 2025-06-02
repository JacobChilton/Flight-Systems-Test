using UnityEngine;

public class propScript : MonoBehaviour
{
    public PlaneTest3 planeTest3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Terrain>() || other.CompareTag("Canyon"))
        {
            Debug.Log("collision");
            planeTest3.deadProp();
        }
    }
}
