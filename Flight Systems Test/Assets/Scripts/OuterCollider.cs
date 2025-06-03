using Unity.VisualScripting;
using UnityEngine;

public class outerCollider : MonoBehaviour
{
    public GameObject outsideObject, sun;
    public Collider innerCollider;  // Re-enable the inner collider

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reactivate the outside object
            if (outsideObject != null)
            {
                outsideObject.SetActive(true);
                sun.SetActive(true);
            }

            // Re-enable inner collider for future use
            if (innerCollider != null)
                innerCollider.enabled = true;

            // Disable this collider
            GetComponent<Collider>().enabled = false;
        }
    }
}
