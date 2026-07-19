using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float lifetime = 0.3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
