using UnityEngine;

public class outerCanyonCollider : MonoBehaviour
{
    public GameObject[] outsideObjects;
    public Collider innerCollider;  // Re-enable the inner collider
    private GameManager gameManager;

    void Start()
    {

        gameManager = GetComponentInParent<GameManager>();
        if (outsideObjects == null || outsideObjects.Length == 0)
        {
            if (gameManager != null)
                outsideObjects = gameManager.canyonWallsOff;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in outsideObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            // Re-enable inner collider for future use
            if (innerCollider != null)
                innerCollider.enabled = true;

            // Disable this collider
            GetComponent<Collider>().enabled = false;
        }
    }
}
