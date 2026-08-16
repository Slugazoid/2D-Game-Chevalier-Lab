using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

// Tempel script ini di GameObject tombol (yang sama dengan komponen Button).
// Efeknya: tombol membesar (scale up) halus pas mouse hover, balik normal pas mouse keluar.
// Opsional bisa juga ganti warna sedikit biar makin kerasa "nyala" pas di-hover.
[RequireComponent(typeof(RectTransform))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Effect")]
    [Tooltip("Seberapa besar tombol membesar pas di-hover, 1 = ukuran normal")]
    public float hoverScale = 1.1f;

    [Tooltip("Durasi animasi scale (detik)")]
    public float scaleDuration = 0.12f;

    [Header("Color Effect (opsional)")]
    [Tooltip("Aktifkan supaya warna tombol berubah pas di-hover")]
    public bool useColorTint = true;

    [Tooltip("Warna tombol pas di-hover")]
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

        // Ambil Graphic dari komponen Button kalau ada (biasanya Image di tombol itu sendiri),
        // supaya efek warna otomatis nempel ke tombolnya tanpa perlu drag manual di Inspector.
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
        // Reset pas tombol dinonaktifkan (misal pas pindah panel), biar gak nyangkut gede/warna aneh.
        if (rectTransform != null) rectTransform.localScale = normalScale;
        if (targetGraphic != null) targetGraphic.color = normalColor;
    }
}
