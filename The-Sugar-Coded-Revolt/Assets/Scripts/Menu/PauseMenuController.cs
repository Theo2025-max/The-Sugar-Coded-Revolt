using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // =================== VOLUME SETTINGS ===================
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null; // I want this to show the current volume
    [SerializeField] private Slider volumeSlider = null;      // I’m adding this so the player can adjust volume
    [SerializeField] private float defaultVolume = 1.0f;

    // =================== GAMEPLAY SETTINGS ===================
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null; // I want this to show current sensitivity
    [SerializeField] private Slider controllerSenSlider = null;      // I’m adding this so player can adjust sensitivity
    [SerializeField] private float defaultSen = 0.47f;
    public float mainControllerSen = 0.47f;

    // =================== CONFIRMATION ===================
    [Header("Confirmation")]
    [SerializeField] private GameObject comfirmationPrompt = null; // I want to show a confirmation when settings are applied

    // =================== PAUSE MENU REFERENCES ===================
    [Header("Pause Menu References")]
    [SerializeField] private GameObject pauseMenuUI = null; // I want this to show/hide the pause panel
    [SerializeField] private GameObject dimOverlay = null;  // I’m adding this to dim the screen while paused

    // =================== PRIVATE VARIABLES ===================
    private bool isPaused = false;
    private MyFirstPersonController fpsController;
    private MyPlayerInput playerInput;

    // =================== UNITY METHODS ===================
    private void Start()
    {
        // Load saved sensitivity
        mainControllerSen = PlayerPrefs.GetFloat("masterSen", defaultSen);

        // Set up slider and text
        controllerSenSlider.minValue = 0.05f;
        controllerSenSlider.maxValue = 1f;
        controllerSenSlider.value = mainControllerSen;
        ControllerSenTextValue.text = mainControllerSen.ToString("0.00");

        // Load saved volume
        float savedVolume = PlayerPrefs.GetFloat("masterVolume", defaultVolume);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;
        volumeTextValue.text = savedVolume.ToString("0.0");

        // Grab player references to enable/disable controls when paused and apply sensitivity
        fpsController = Object.FindFirstObjectByType<MyFirstPersonController>();
        playerInput = Object.FindFirstObjectByType<MyPlayerInput>();

        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen; // Apply sensitivity immediately

        // Start with pause menu hidden
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (dimOverlay != null) dimOverlay.SetActive(false);
    }

    private void Update()
    {
        // I want pressing P to toggle pause menu
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

        Time.timeScale = 0f; // I want the gameplay to stop while paused

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

        Time.timeScale = 1f; // I want the gameplay to resume

        if (fpsController != null) fpsController.enabled = true;
        if (playerInput != null) playerInput.cursorAffectsLook = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // =================== BUTTON METHODS ===================
    public void OnResumeButton()
    {
        ResumeGame(); // I want this button to resume the game
    }

    public void OnRestartButton()
    {
        ResumeGame(); // reset time scale before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // I want this to restart the current level
    }

    public void OnQuitToMainMenuButton()
    {
        // Reset time scale and controls
        Time.timeScale = 1f;
        if (fpsController != null) fpsController.enabled = true;
        if (playerInput != null) playerInput.cursorAffectsLook = true;

        // Force cursor visible/unlocked for main menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Load main menu scene
        SceneManager.LoadScene("Main Menu");
    }

    // =================== VOLUME & SENSITIVITY CONTROLS ===================
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // I want volume changes to apply immediately
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

        // I want sensitivity changes to apply immediately
        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen;
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetFloat("masterSen", mainControllerSen);

        if (fpsController != null)
            fpsController.rotationSpeed = mainControllerSen; // Apply immediately

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
        comfirmationPrompt.SetActive(true); // I want this to briefly show confirmation when settings apply
        yield return new WaitForSeconds(2);
        comfirmationPrompt.SetActive(false);
    }
}
