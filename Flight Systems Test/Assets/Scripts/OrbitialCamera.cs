using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField, Range(1f, 20f)] private float distance = 8f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float sensitivity = 1f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    private float yaw = 0f;
    private float pitch = 15f;
    private float zoomDelta = 0f;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Flight.Enable();
        controls.Flight.Zoom.performed += OnZoom;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        controls.Flight.Zoom.performed -= OnZoom;
        controls.Flight.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnZoom(InputAction.CallbackContext ctx)
    {
        zoomDelta = ctx.ReadValue<float>();
    }

    private void LateUpdate()
    {
        // Apply zoom input
        distance -= zoomDelta * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        zoomDelta = 0f; // reset after applying

        // Mouse look
        Vector2 mouse = Mouse.current.delta.ReadValue();
        yaw += mouse.x * sensitivity;
        pitch -= mouse.y * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        transform.position = pivot.position + offset;
        transform.rotation = rotation;
    }
}
