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

        if (enemiesLeft <= 0 && isPlayerAlive)
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
        youWinText.SetActive(true);
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
}
