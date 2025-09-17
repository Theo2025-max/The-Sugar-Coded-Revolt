using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    // ------------------- UI REFERENCES -------------------
    public Image fadePanel;         // Black panel covering the screen
    public float fadeDuration = 1f; // Duration of fade in/out

    private bool isFading = false;  // Prevent double clicks

    // ------------------- MAKE FADE PANEL PERSISTENT -------------------
    private void Awake()
    {
        // Keep this object alive between scenes
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Start fully transparent
        if (fadePanel != null)
            fadePanel.color = new Color(0, 0, 0, 0);
    }

    // ------------------- TRIGGER FADE -------------------
    public void StartGame()
    {
        if (!isFading)
            StartCoroutine(FadeOutInScene("Level 1")); // <-- Correct scene name with space
    }

    // ------------------- FADE SEQUENCE -------------------
    private IEnumerator FadeOutInScene(string sceneName)
    {
        isFading = true;

        // Block UI input during fade
        fadePanel.raycastTarget = true;

        // --- Fade Out ---
        yield return StartCoroutine(Fade(0f, 1f));

        // --- Load Level 1 asynchronously ---
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // --- Fade In ---
        yield return StartCoroutine(Fade(1f, 0f));

        // Unlock UI input
        fadePanel.raycastTarget = false;
        isFading = false;
    }

    // ------------------- FADE HELPER -------------------
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadePanel == null)
            yield break;

        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadePanel.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // Ensure final alpha is exact
        fadePanel.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
