using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 5f;

    [SerializeField] private float returnSpeed = 2f;
    [SerializeField] private Transform pivot;
    [SerializeField] private PlayerControls controls;

    private Vector2 currentRotation;
    private bool isLooking;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Flight.FreeLookToggle.performed += _ => isLooking = true;
        controls.Flight.FreeLookToggle.canceled += _ => isLooking = false;
    }

    private void OnEnable() => controls.Flight.Enable();
    private void OnDisable() => controls.Flight.Disable();

    private void Update()
    {
        if (isLooking)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            currentRotation.x += mouseDelta.x * mouseSensitivity * Time.deltaTime;
            currentRotation.y -= mouseDelta.y * mouseSensitivity * Time.deltaTime;
            currentRotation.y = Mathf.Clamp(currentRotation.y, -80f, 80f);
        }
        else
        {
            currentRotation = Vector2.Lerp(currentRotation, Vector2.zero, Time.deltaTime * returnSpeed);
        }

        OrbitCamera();
    }

    private void OrbitCamera()
    {
        Quaternion rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        Vector3 offset = rotation * Vector3.back;

        transform.position = pivot.position + offset * 1f; // Orbit distance
        transform.LookAt(pivot);
    }
}
