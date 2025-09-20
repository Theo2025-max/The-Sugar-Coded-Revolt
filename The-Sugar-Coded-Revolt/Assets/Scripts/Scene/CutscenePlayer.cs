using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class CutscenePlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "MainMenu";

    [Header("Audio")]
    [SerializeField] private AudioClip overrideAudioClip;
    [Range(0f, 1f)]
    [SerializeField] private float audioVolume = 1f;

    [Header("Fade")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Skip Text Settings")]
    [SerializeField] private TMP_Text skipText;
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    [Header("Pulse Animation")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.1f;

    [Header("Jiggle Animation")]
    [SerializeField] private float jiggleDuration = 0.2f;
    [SerializeField] private float jiggleScaleAmount = 0.2f;
    [SerializeField] private float jiggleRotationAmount = 15f;
    [SerializeField] private bool enableColorFlash = true;
    [SerializeField] private Color flashColor = Color.yellow;

    private bool isTransitioning = false;
    private bool isJiggling = false;
    private Color originalColor;

    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        videoPlayer.loopPointReached += OnVideoFinished;

        if (skipText != null)
            originalColor = skipText.color;
    }

    private IEnumerator Start()
    {
        
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeFromBlack());
        }

        if (allowSkip && skipText != null)
            skipText.gameObject.SetActive(true);

        videoPlayer.Play();

        if (overrideAudioClip != null && audioSource != null)
        {
            audioSource.clip = overrideAudioClip;
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
    }

    private void Update()
    {
        // Pulse animation
        if (skipText != null && !isJiggling)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            skipText.transform.localScale = new Vector3(scale, scale, 1f);
        }

        // Keyboard skip
        if (allowSkip && !isTransitioning && Input.GetKeyDown(skipKey))
        {
            StartCoroutine(TransitionToMenu());
        }

        // Mouse click skip
        if (allowSkip && !isTransitioning && skipText != null && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(skipText.rectTransform, mousePos))
            {
                if (!isJiggling)
                    StartCoroutine(JiggleSkip());

                StartCoroutine(TransitionToMenu());
            }
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(TransitionToMenu());
    }

    private IEnumerator TransitionToMenu()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        // Fade out audio
        if (audioSource != null && audioSource.isPlaying)
        {
            float startVol = audioSource.volume;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
                yield return null;
            }
            audioSource.Stop();
        }

        videoPlayer.Stop();

        // Fade out screen
        if (fadeImage != null)
            yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeToBlack()
    {
        fadeImage.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }

    private IEnumerator JiggleSkip()
    {
        isJiggling = true;

        Vector3 originalScale = skipText.transform.localScale;
        Quaternion originalRotation = skipText.transform.rotation;

        float elapsed = 0f;

        while (elapsed < jiggleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / jiggleDuration;

            // Scale jiggle
            float scale = 1f + Mathf.Sin(t * Mathf.PI * 4) * jiggleScaleAmount;
            skipText.transform.localScale = new Vector3(scale, scale, 1f);

            // Rotation jiggle
            float rotZ = Mathf.Sin(t * Mathf.PI * 4) * jiggleRotationAmount;
            skipText.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

            // color flash
            if (enableColorFlash)
            {
                float flashT = Mathf.Sin(t * Mathf.PI * 4) * 0.5f + 0.5f;
                skipText.color = Color.Lerp(originalColor, flashColor, flashT);
            }

            yield return null;
        }

        // Reset
        skipText.transform.localScale = originalScale;
        skipText.transform.rotation = originalRotation;
        skipText.color = originalColor;

        isJiggling = false;
    }
}
