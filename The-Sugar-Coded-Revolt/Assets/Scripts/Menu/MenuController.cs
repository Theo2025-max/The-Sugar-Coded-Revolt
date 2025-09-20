using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.ProBuilder.MeshOperations;

public class MenuController : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public bool onReturnToMenu;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        if (Object.FindObjectsByType<MenuController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1.5f);
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);

        controllerSenSlider.minValue = 0.05f;
        controllerSenSlider.maxValue = 1f;
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        float savedVolume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");

        // Ensure cursor is free when in menus
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

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
            fadeEffect.ScreenFade(1, 1.5f);
            yield return new WaitForSeconds(1.5f);
        }

        SceneManager.LoadScene(sceneName);

        yield return null;

        fadeEffect = Object.FindFirstObjectByType<UI_FadeEffect>();

        if (fadeEffect != null)
        {
            fadeEffect.ScreenFade(0, 1.5f);
        }
    }

    public void QuitButton()
    {
        Debug.Log("Peace out, Theo! Application quitting.");
        Application.Quit();
    }

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

    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
