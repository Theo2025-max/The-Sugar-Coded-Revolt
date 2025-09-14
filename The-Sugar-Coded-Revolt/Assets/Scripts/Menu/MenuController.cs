using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MenuController : MonoBehaviour
{
    // ------------------- Volume Settings -------------------
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    // ------------------- Gameplay Settings -------------------
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    // ------------------- Toggle Settings -------------------
    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    // ------------------- Confirmation -------------------
    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    // ------------------- Levels To Load -------------------
    [Header("Levels To Load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog = null;

    private void Start()
    {
        // Load saved sensitivity
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);

        // Slider setup
        controllerSenSlider.minValue = 0.05f; // min value
        controllerSenSlider.maxValue = 1f;    // max value
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.000");
    }

    // ------------------- Scene Management -------------------
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }

    // ------------------- Quit Game -------------------
    public void QuitButton()
    {
        Debug.Log("Peace out, Theo! The app has taken a nap.");
        Application.Quit();
    }

    // ------------------- Volume Controls -------------------
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

    // ------------------- Sensitivity Controls -------------------
    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.Clamp(sensitivity, 0.05f, 1f); // clamp
        ControllerSenTextValue.text = mainControllerSen.ToString("0.000");

        // Save to PlayerPrefs for persistence
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
            PlayerPrefs.SetFloat("masterInvertY", 1);
        else
            PlayerPrefs.SetFloat("masterInvertY", 0);

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    // ------------------- Reset Button -------------------
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
            ControllerSenTextValue.text = defaultSen.ToString("0.000");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertYToggle.isOn = false;

            GameplayApply();
        }
    }

    // ------------------- Confirmation Box -------------------
    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
