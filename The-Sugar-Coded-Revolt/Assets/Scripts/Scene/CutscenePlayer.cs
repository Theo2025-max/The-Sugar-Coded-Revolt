using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CutscenePlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer; // my cutscene video player
    [SerializeField] private AudioSource audioSource; // audio for the cutscene

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "MainMenu"; // scene to load after cutscene

    [Header("Audio")]
    [SerializeField] private AudioClip overrideAudioClip; // optional audio clip
    [Range(0f, 1f)]
    [SerializeField] private float audioVolume = 1f;

    [Header("Fade")]
    [SerializeField] private Image fadeImage; // black image for fade in/out
    [SerializeField] private float fadeDuration = 1f;

    [Header("Skip")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;
    [SerializeField] private TMP_Text skipText; // text that acts as skip button
    [SerializeField] private float skipPulseSpeed = 2f;
    [SerializeField] private float skipPulseAmount = 0.1f;

    private bool isTransitioning = false; // prevent double transitions

    private void Awake()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        videoPlayer.loopPointReached += OnVideoFinished;

        // make sure we start black
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        if (skipText != null)
            skipText.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        // fade from black into video
        if (fadeImage != null)
            yield return StartCoroutine(FadeFromBlack());

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
        // skip via keyboard
        if (allowSkip && !isTransitioning && Input.GetKeyDown(skipKey))
            StartCoroutine(TransitionToMenu());

        // skip via clicking skip text
        if (allowSkip && !isTransitioning && skipText != null && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.RectangleContainsScreenPoint(skipText.rectTransform, mousePos))
                StartCoroutine(TransitionToMenu());
        }

        // simple pulse effect for skip text
        if (skipText != null && skipText.gameObject.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.time * skipPulseSpeed) * skipPulseAmount;
            skipText.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(TransitionToMenu()); // move to menu when video ends
    }

    private IEnumerator TransitionToMenu()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        if (skipText != null)
            skipText.gameObject.SetActive(false);

        // fade out audio smoothly
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
}
