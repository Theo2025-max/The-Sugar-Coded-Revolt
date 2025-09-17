using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;

    // ------------------- Keep this alive across scenes -------------------
    private void Awake()
    {
        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        // If there's already another MenuController, I don't need this one
        if (Object.FindObjectsByType<MenuController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Make sure this MenuController sticks around when I change scenes
        DontDestroyOnLoad(gameObject);
    }

    // ------------------- Volume stuff -------------------
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    // ------------------- Gameplay settings -------------------
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    // ------------------- Confirmation prompt -------------------
    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    // ------------------- Levels I can load -------------------
    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    // ------------------- Set everything up when scene starts -------------------
    private void Start()
    {
        if (fadeEffect != null)
            fadeEffect.ScreenFade(0, 1.5f);

        // Load the saved controller sensitivity
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);

        // Make the slider work properly
        controllerSenSlider.minValue = 0.05f; // minimum
        controllerSenSlider.maxValue = 1f;    // maximum
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        // Set up volume based on saved value
        float savedVolume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");
    }

    // ------------------- Scene changing stuff -------------------
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
            // Fade the screen to black before changing
            fadeEffect.ScreenFade(1, 1.5f);
            yield return new WaitForSeconds(1.5f);
        }

        // Actually load the new scene
        SceneManager.LoadScene(sceneName);

        // Wait a frame so Unity finishes loading
        yield return null;

        // Grab the fade effect in the new scene
        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        if (fadeEffect != null)
        {
            // Fade back in so it doesn't stay black
            fadeEffect.ScreenFade(0, 1.5f);
        }
    }

    // ------------------- Quit the game -------------------
    public void QuitButton()
    {
        Debug.Log("Peace out, Theo! Application quitting.");
        Application.Quit();
    }

    // ------------------- Volume controls -------------------
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

    // ------------------- Sensitivity controls -------------------
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

    // ------------------- Reset buttons -------------------
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

    // ------------------- Show confirmation message for a bit -------------------
    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
