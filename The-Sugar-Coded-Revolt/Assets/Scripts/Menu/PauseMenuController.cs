using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // =================== VOLUME SETTINGS ===================
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    // =================== GAMEPLAY SETTINGS ===================
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    // =================== CONFIRMATION ===================
    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null;

    // =================== PAUSE MENU REFERENCES ===================
    [Header("Pause Menu References")]
    [SerializeField] private GameObject pauseMenuUI = null;
    [SerializeField] private GameObject dimOverlay = null;

    // =================== PRIVATE VARIABLES ===================
    private bool isPaused = false;
    private MyFirstPersonController fpsController;
    private MyPlayerInput playerInput;

    // =================== UNITY METHODS ===================
    private void Start()
    {
        
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);
        controllerSenSlider.minValue = 0.05f;
        controllerSenSlider.maxValue = 1f;
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        
        float savedVolume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");

        
        fpsController = Object.FindFirstObjectByType<MyFirstPersonController>();
        playerInput = Object.FindFirstObjectByType<MyPlayerInput>();

        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen;

        
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dimOverlay != null) dimOverlay.SetActive(false);
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // =================== PAUSE/RESUME LOGIC ===================
    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (dimOverlay != null) dimOverlay.SetActive(true);

        Time.timeScale = 0f;

        if (fpsController != null) fpsController.enabled = false;
        if (playerInput != null) playerInput.cursorAffectsLook = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dimOverlay != null) dimOverlay.SetActive(false);

        Time.timeScale = 1f;

        if (fpsController != null) fpsController.enabled = true;
        if (playerInput != null) playerInput.cursorAffectsLook = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // =================== BUTTON METHODS ===================
    public void OnResumeButton() => ResumeGame();

    public void OnRestartButton()
    {
        ResumeGame();
        UI_FadeEffect fade = Object.FindFirstObjectByType<UI_FadeEffect>();
        if (fade != null)
        {
            fade.ScreenFade(1f, 1f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnQuitToMainMenuButton()
    {
        Time.timeScale = 1f;
        if (fpsController != null) fpsController.enabled = true;
        if (playerInput != null) playerInput.cursorAffectsLook = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UI_FadeEffect fade = Object.FindFirstObjectByType<UI_FadeEffect>();
        if (fade != null)
        {
            fade.ScreenFade(1f, 1f, () =>
            {
                SceneManager.LoadScene("Main Menu");
            });
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    // =================== VOLUME & SENSITIVITY ===================
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

        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen;
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);

        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen;

        StartCoroutine(ConfirmationBox());
    }

    // =================== RESET BUTTON ===================
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }
        else if (MenuType == "Gameplay")
        {
            ControllerSenTextValue.text = defaultSen.ToString("0.00");
            controllerSenSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            GameplayApply();
        }
    }

    // =================== CONFIRMATION BOX ===================
    public IEnumerator ConfirmationBox()
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
