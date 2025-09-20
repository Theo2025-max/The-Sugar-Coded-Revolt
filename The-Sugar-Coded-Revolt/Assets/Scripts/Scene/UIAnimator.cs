using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class UIAnimator : MonoBehaviour, IPointerClickHandler
{
    [Header("Pulse Settings")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;

    [Header("Jiggle Settings")]
    [SerializeField] private float jiggleDuration = 0.2f;
    [SerializeField] private float jiggleScaleAmount = 0.2f;
    [SerializeField] private float jiggleRotationAmount = 15f;

    [Header("Color Flash")]
    [SerializeField] private bool enableColorFlash = true;
    [SerializeField] private Color flashColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.2f;

    private TMP_Text tmpText;
    private bool isJiggling = false;
    private Color originalColor;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        if (tmpText != null)
            originalColor = tmpText.color;
    }

    private void Update()
    {
        // Pulse animation
        if (enablePulse && !isJiggling && tmpText != null)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            tmpText.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    // Event-driven click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isJiggling)
            StartCoroutine(JiggleAndFlash());
    }

    private IEnumerator JiggleAndFlash()
    {
        isJiggling = true;

        Vector3 originalScale = tmpText.transform.localScale;
        Quaternion originalRotation = tmpText.transform.rotation;
        float elapsed = 0f;

        while (elapsed < jiggleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / jiggleDuration;

            // Jiggle scale
            float scale = 1f + Mathf.Sin(t * Mathf.PI * 4) * jiggleScaleAmount;
            tmpText.transform.localScale = new Vector3(scale, scale, 1f);

            // Jiggle rotation
            float rotZ = Mathf.Sin(t * Mathf.PI * 4) * jiggleRotationAmount;
            tmpText.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            // Optional color flash
            if (enableColorFlash)
            {
                float flashT = Mathf.Sin(t * Mathf.PI * 4) * 0.5f + 0.5f;
                tmpText.color = Color.Lerp(originalColor, flashColor, flashT);
            }

            yield return null;
        }

        // Reset to original
        tmpText.transform.localScale = originalScale;
        tmpText.transform.rotation = originalRotation;
        tmpText.color = originalColor;

        isJiggling = false;
    }
}
