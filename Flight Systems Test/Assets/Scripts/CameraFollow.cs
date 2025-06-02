using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target => target;

    [SerializeField] private Transform target;
    [SerializeField] private PlaneTest3 planeController;

    [Header("Follow Settings")]
    [SerializeField, Range(0.01f, 0.5f)] private float cameraHeight = 0.25f;
    [SerializeField, Range(1.0f, 10.0f)] private float rotationSpeed = 3.0f;
    [SerializeField, Range(0.01f, 1.0f)] private float smoothTime = 0.2f;

    [Header("Distance Settings")]
    [SerializeField] private bool autoAdjustDistance = false;
    [SerializeField, Range(1.0f, 7.0f)] private float distance = 7f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 7f;
    [SerializeField] private float maxSpeed = 220f;

    private Vector3 velocity;
    private Transform t;

    private float angle;
    private float currentDistance;

    private void Awake()
    {
        t = transform;

        if (planeController == null && target != null)
            planeController = target.GetComponent<PlaneTest3>();

        currentDistance = autoAdjustDistance ? maxDistance : distance;
    }

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        float targetDistance = distance;

        if (autoAdjustDistance && planeController != null)
        {
            float speedFactor = Mathf.Clamp01(planeController.airspeed / maxSpeed);
            targetDistance = Mathf.Lerp(maxDistance, minDistance, speedFactor);
        }

        if (autoAdjustDistance)
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.fixedDeltaTime * 5f);
        else
            currentDistance = targetDistance;

        Vector3 localOffset = (-Vector3.forward * currentDistance) + (Vector3.up * currentDistance * cameraHeight);
        Vector3 desiredPos = target.TransformPoint(localOffset);

        t.position = Vector3.SmoothDamp(t.position, desiredPos, ref velocity, smoothTime);

        angle = Mathf.Abs(Quaternion.Angle(t.rotation, target.rotation));
        t.rotation = Quaternion.RotateTowards(
            t.rotation, target.rotation, (angle * rotationSpeed) * Time.fixedDeltaTime);
    }
}
