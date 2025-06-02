using UnityEngine;

public class CustomizationController : MonoBehaviour
{
    public GameObject customizationUI; // UI for changing colours
    public GameObject[] colorOptions, flightCameras, shopCameras; // Different plane color objects
    
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
        PlayerPrefs.SetInt("SelectedPlane", currentIndex);
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (!isCustomizing) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeColor(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeColor(1);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCustomization();
        }
        cameraTimer += Time.unscaledDeltaTime; // Use unscaled time since game is paused
        if (cameraTimer >= cameraSwitchInterval)
        {
            CycleCamera(1);
            cameraTimer = 0f;
        }
    }

    void ChangeColor(int direction)
    {
        colorOptions[currentIndex].SetActive(false);
        currentIndex = (currentIndex + direction + colorOptions.Length) % colorOptions.Length;
        colorOptions[currentIndex].SetActive(true);
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
