using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] TMP_Text enemiesLeftText;
    [SerializeField] GameObject youWinText;

    int enemiesLeft = 0;

    const string ENEMIES_LEFT_STRING = "Enemies Left: ";

    private void Awake()
    {
        if(instance != null & instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void AdjustEnemiesLeft(int amount)
    {
        enemiesLeft += amount;
        string enemiesLeftTMP = enemiesLeft.ToString();
        enemiesLeftText.text = ENEMIES_LEFT_STRING + enemiesLeftTMP;
        //Debug.Log(ENEMIES_LEFT_STRING + enemiesLeftText.ToString());

        if (enemiesLeft <= 0)
        {
            youWinText.SetActive(true);
        }
    }
    public void RestartLevelButton()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }



    public void QuitButton()
    {
        Debug.LogWarning("Theo, this does not work in the Unity Editor! ");
        Application.Quit();
    }
}
