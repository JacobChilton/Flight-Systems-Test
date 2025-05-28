using UnityEngine;

public class innerCollider : MonoBehaviour
{
    public GameObject outsideObject;        //Object to deactivate - Canyon
    public Collider outerCollider;          //The outer collider to enable
    public Collider thisCollider;           //Optional: if not using 'GetComponent'

    void Start()
    {
        if (thisCollider == null)
            thisCollider = GetComponent<Collider>();

        // Ensure the inner collider is set as trigger
        if (!thisCollider.isTrigger)
            thisCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the player has the "Player" tag
        {
            // Deactivate the outside object
            if (outsideObject != null)
                outsideObject.SetActive(false);

            // Disable this (inner) collider
            thisCollider.enabled = false;

            // Enable the outer collider
            if (outerCollider != null)
                outerCollider.enabled = true;
        }
    }
}

