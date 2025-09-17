using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;

    // ------------------- PERSISTENCE ACROSS SCENES -------------------
    private void Awake()
    {
        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        // If another MenuController already exists, destroy this one
        if (Object.FindObjectsByType<MenuController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Keep this MenuController alive across scenes
        DontDestroyOnLoad(gameObject);
    }

    // ------------------- VOLUME SETTINGS -------------------
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    // ------------------- GAMEPLAY SETTINGS -------------------
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    // ------------------- CONFIRMATION -------------------
    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    // ------------------- LEVELS TO LOAD -------------------
    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    // ------------------- INITIALIZATION -------------------
    private void Start()
    {
        if (fadeEffect != null)
            fadeEffect.ScreenFade(0, 1.5f);

        // Load saved sensitivity
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);

        // Slider setup
        controllerSenSlider.minValue = 0.05f; // clamp min
        controllerSenSlider.maxValue = 1f;    // clamp max
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        // Volume setup
        float savedVolume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");
    }

    // ------------------- SCENE MANAGEMENT -------------------
    public void NewGameDialogYes()
    {
        StartCoroutine(LoadSceneWithFade(_newGameLevel));
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            StartCoroutine(LoadSceneWithFade(levelToLoad));
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        if (fadeEffect != null)
        {
            // Fade out to black
            fadeEffect.ScreenFade(1, 1.5f);
            yield return new WaitForSeconds(1.5f);
        }

        // Load the scene
        SceneManager.LoadScene(sceneName);

        // Wait one frame to let Unity finish loading
        yield return null;

        // Re-link fadeEffect in the new scene
        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        if (fadeEffect != null)
        {
            // Fade back in from black
            fadeEffect.ScreenFade(0, 1.5f);
        }
    }

    // ------------------- QUIT GAME -------------------
    public void QuitButton()
    {
        Debug.Log("Peace out, Theo! Application quitting.");
        Application.Quit();
    }

    // ------------------- VOLUME CONTROLS -------------------
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    // ------------------- SENSITIVITY CONTROLS -------------------
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.Clamp(sensitivity, 0.05f, 1f);
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    // ------------------- RESET BUTTON -------------------
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            ControllerSenTextValue.text = defaultSen.ToString("0.00");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            GameplayApply();
        }
    }

    // ------------------- CONFIRMATION BOX -------------------
    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
