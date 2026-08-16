using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Effect")]
    public float hoverScale = 1.1f;

    [Tooltip("Durasi animasi scale (detik)")]
    public float scaleDuration = 0.12f;

    [Header("Color Effect (opsional)")]
    public bool useColorTint = true;
    public Color hoverColor = new Color(1f, 0.85f, 0.4f, 1f);

    private RectTransform rectTransform;
    private Vector3 normalScale;
    private Graphic targetGraphic;
    private Color normalColor;
    private Coroutine scaleCoroutine;
    private Coroutine colorCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        normalScale = rectTransform.localScale;

        targetGraphic = GetComponent<Graphic>();
        if (targetGraphic != null)
        {
            normalColor = targetGraphic.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScale(normalScale * hoverScale);
        if (useColorTint && targetGraphic != null)
        {
            StartColor(hoverColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScale(normalScale);
        if (useColorTint && targetGraphic != null)
        {
            StartColor(normalColor);
        }
    }

    private void StartScale(Vector3 target)
    {
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(target));
    }

    private void StartColor(Color target)
    {
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        colorCoroutine = StartCoroutine(ColorTo(target));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 start = rectTransform.localScale;
        float timer = 0f;

        while (timer < scaleDuration)
        {
            timer += Time.unscaledDeltaTime;
            rectTransform.localScale = Vector3.Lerp(start, target, timer / scaleDuration);
            yield return null;
        }
        rectTransform.localScale = target;
    }

    private IEnumerator ColorTo(Color target)
    {
        Color start = targetGraphic.color;
        float timer = 0f;

        while (timer < scaleDuration)
        {
            timer += Time.unscaledDeltaTime;
            targetGraphic.color = Color.Lerp(start, target, timer / scaleDuration);
            yield return null;
        }
        targetGraphic.color = target;
    }

    private void OnDisable()
    {
        if (rectTransform != null) rectTransform.localScale = normalScale;
        if (targetGraphic != null) targetGraphic.color = normalColor;
    }
}
