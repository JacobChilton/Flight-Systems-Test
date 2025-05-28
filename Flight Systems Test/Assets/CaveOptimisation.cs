using UnityEngine;

public class AreaActivationTrigger : MonoBehaviour
{
    public GameObject objectToToggle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToToggle != null)
                objectToToggle.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (objectToToggle != null)
                objectToToggle.SetActive(false);
        }
    }
}
