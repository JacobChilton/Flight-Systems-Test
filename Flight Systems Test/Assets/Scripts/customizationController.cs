using UnityEditor.Rendering;
using UnityEngine;

public class CustomizationController : MonoBehaviour
{
    public GameObject customizationUI, flightUI; // UI for changing colours
    public GameObject[] colorOptions, flightCameras, shopCameras; // Different plane color objects
    public Transform propeller;
    
    private int currentIndex = 0;
    private bool isCustomizing = false;
    public float cameraSwitchInterval = 5f; // Time in seconds between switches
    private float cameraTimer = 0f;
    private int currentCamIndex = 0; // Add this to your variables at the top

    void Start()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedPlane", 0); // Default to 0
        for (int i = 0; i < colorOptions.Length; i++)
        {
            colorOptions[i].SetActive(i == savedIndex);
        }
        currentIndex = savedIndex;
        if (currentIndex == 3)
        {
            currentIndex--;
            ChangeColor(1);
        }
    }
    public void EnterCustomization()
    {
        for (int i = 0; i < flightCameras.Length; i++)
        {
            flightCameras[i].SetActive(false);
        }
        currentCamIndex = 0;
        for (int i = 0; i < shopCameras.Length; i++)
        {
            shopCameras[i].SetActive(i == 0);
        }
        cameraTimer = 0f;
        Cursor.visible = true;
        isCustomizing = true;
        customizationUI.SetActive(true);
        flightUI.SetActive(false);
        //Time.timeScale = 0; // Optional: Pause game
    }

    public void ExitCustomization()
    {
        Cursor.visible = false;
        isCustomizing = false;
        customizationUI.SetActive(false);
        //Time.timeScale = 1;
        for (int i = 0; i < shopCameras.Length; i++)
        {
            shopCameras[i].SetActive(false);
        }
        for (int i = 0; i < flightCameras.Length; i++)
        {
            flightCameras[i].SetActive(true);
        }
        flightUI.SetActive(true);
        customizationUI.SetActive(false);
        PlayerPrefs.SetInt("SelectedPlane", currentIndex);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (!isCustomizing) return;
        if (cameraTimer >= cameraSwitchInterval)
        {
            CycleCamera(1);
            cameraTimer = 0f;
        }
    }

    public void ChangeColor(int direction)
    {
        colorOptions[currentIndex].SetActive(false);
        currentIndex = (currentIndex + direction + colorOptions.Length) % colorOptions.Length;
        colorOptions[currentIndex].SetActive(true);
        if (currentIndex == 3) {
            propeller.localPosition = new Vector3(0.023023814f, 0.0651969835f, 1.55971265f);
        }
        else
        {
            propeller.localPosition = new Vector3(0.0229406059f, 0.0651957467f, 1.66531634f);
        }
    }
    void CycleCamera(int direction)
    {
        if (shopCameras.Length == 0) return;

        // Deactivate current cam
        shopCameras[currentCamIndex].SetActive(false);

        // Increment or decrement index safely
        currentCamIndex = (currentCamIndex + direction + shopCameras.Length) % shopCameras.Length;

        // Activate the new cam
        shopCameras[currentCamIndex].SetActive(true);
    }
}
