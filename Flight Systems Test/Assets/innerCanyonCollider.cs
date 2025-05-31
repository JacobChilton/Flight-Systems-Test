using UnityEngine;

public class innerCanyonCollider : MonoBehaviour
{
    public GameObject[] outsideObjects;        //Object to deactivate - Canyon
    public Collider outerCollider;          //The outer collider to enable
    public Collider thisCollider;           //Optional: if not using 'GetComponent'
    private GameManager gameManager;

    void Start()
    {
        if (thisCollider == null)
            thisCollider = GetComponent<Collider>();

        // Ensure the inner collider is set as trigger
        if (!thisCollider.isTrigger)
            thisCollider.isTrigger = true;

        gameManager = GetComponentInParent<GameManager>();
        if (outsideObjects == null || outsideObjects.Length == 0)
        {
            if (gameManager != null)
                outsideObjects = gameManager.canyonWallsOff;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {

            foreach (var obj in outsideObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            // Disable this (inner) collider
            thisCollider.enabled = false;

            // Enable the outer collider
            if (outerCollider != null)
                outerCollider.enabled = true;
        }
    }
}
