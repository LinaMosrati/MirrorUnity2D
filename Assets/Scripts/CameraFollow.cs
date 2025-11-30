using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Player to follow
    public float smoothSpeed = 5f; // Smoothing speed
    public Vector3 offset;         // Offset from the target

    void Start()
    {
        if (target != null)
        {
            // Initialize the camera at the correct position at game start
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }
}
