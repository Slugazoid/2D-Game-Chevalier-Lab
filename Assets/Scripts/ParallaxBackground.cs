using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [Range(0f, 1f)][SerializeField] private float parallaxFactorX = 0.5f;
    [Range(0f, 1f)][SerializeField] private float parallaxFactorY = 0f;
    private Vector3 lastCameraPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(delta.x * parallaxFactorX, delta.y * parallaxFactorY, 0f);
        lastCameraPosition = cameraTransform.position;
    }
}
