using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class NeonFlicker : MonoBehaviour
{
    [SerializeField] private float baseIntensity = 1.3f;
    [SerializeField] private float minIntensity = 0.3f;

    [SerializeField] private float flickerIntervalMin = 0.05f;
    [SerializeField] private float flickerIntervalMax = 3f;
    [SerializeField] private float flickerDuration = 0.08f;
    [SerializeField] private bool randomFlickerMode = true;

    private Light2D light2D;
    private float timer;
    private float nextFlickerTime;


    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        light2D.intensity = baseIntensity;
        SetNextFlickerTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (randomFlickerMode)
        {
            RandomFlicker();
        }
        else
        {
            SmoothFlicker();
        }
    }

    private void RandomFlicker()
    {
        timer += Time.deltaTime;

        if (timer >= nextFlickerTime)
        {
            // Kedip sebentar (redup), lalu balik normal
            light2D.intensity = minIntensity;
            Invoke(nameof(ResetIntensity), flickerDuration);

            timer = 0f;
            SetNextFlickerTime();
        }
    }

    private void ResetIntensity()
    {
        light2D.intensity = baseIntensity;
    }

    private void SmoothFlicker()
    {
        // Naik-turun halus pakai Perlin noise, kesan "buzz" listrik tidak stabil
        float noise = Mathf.PerlinNoise(Time.time * 5f, 0f);
        light2D.intensity = Mathf.Lerp(minIntensity, baseIntensity, noise);
    }

    private void SetNextFlickerTime()
    {
        nextFlickerTime = Random.Range(flickerIntervalMin, flickerIntervalMax);
    }
}
