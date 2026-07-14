using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset;
    public float smoothSpeed = 0.9f;

    public Camera cam;
    public Transform LeftBound;
    public Transform RightBound;

    private void LateUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        float camHalfWidth = cam.orthographicSize * cam.aspect;
        float clampedX = Mathf.Clamp(
            desiredPosition.x,
            LeftBound.position.x + camHalfWidth,
            RightBound.position.x + camHalfWidth
            );
        desiredPosition.x = clampedX;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
