using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("UI References")]
    [SerializeField] TMP_Text enemiesLeftText;
    [SerializeField] GameObject youWinText;

    [Header("Fade Settings")]
    [SerializeField] UI_FadeEffect fadeEffect;
    [SerializeField] float startFadeDuration = 2f;
    [SerializeField] float winDelay = 2f;
    [SerializeField] float fadeDuration = 2f;
    [SerializeField] string creditsSceneName = "Credits";

    private int enemiesLeft = 0;
    private bool isPlayerAlive = true;
    private bool hasWon = false;   // Prevent multiple win triggers

    const string ENEMIES_LEFT_STRING = "Enemies Left: ";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateEnemiesLeftUI();
        fadeEffect.ScreenFade(0f, startFadeDuration);
    }

    public void AdjustEnemiesLeft(int amount)
    {
        enemiesLeft += amount;
        UpdateEnemiesLeftUI();

        // Trigger win only if all enemies are gone AND player is alive
        if (enemiesLeft <= 0 && isPlayerAlive && !hasWon)
        {
            HandleWinCondition();
        }
    }

    private void UpdateEnemiesLeftUI()
    {
        enemiesLeftText.text = ENEMIES_LEFT_STRING + enemiesLeft.ToString();
    }

    private void HandleWinCondition()
    {
        hasWon = true;
        youWinText.SetActive(true);

        // Heal player to full with animation after a short delay
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.HealToFull(0.5f, 0.2f); // 0.5s delay before start, 0.2s per shield
        }

        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(winDelay);

        fadeEffect.ScreenFade(1f, fadeDuration, () =>
        {
            SceneManager.LoadScene(creditsSceneName);
        });
    }

    // ------------------- PLAYER STATUS -------------------
    public void PlayerDied()
    {
        if (hasWon) return; // Prevent dying after winning
        isPlayerAlive = false;
    }

    // ------------------- BUTTON METHODS -------------------
    public void RestartLevelButton()
    {
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        fadeEffect.ScreenFade(1f, fadeDuration, () =>
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        });
        yield return null;
    }

    public void QuitButton()
    {
        Debug.LogWarning("Theo, this does not work in the Unity Editor!");
        Application.Quit();
    }

    // ------------------- UTILITY -------------------
    public bool HasPlayerWon()
    {
        return hasWon;
    }
}
