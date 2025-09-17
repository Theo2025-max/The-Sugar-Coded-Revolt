using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // ------------------- PLAYER SETTINGS -------------------
    [Range(1, 10)]
    [SerializeField] int startingHealth = 5;

    [SerializeField] CinemachineCamera deathVirtualCamera;
    [SerializeField] Transform weaponCamera;
    [SerializeField] Image[] shieldBars;
    [SerializeField] GameObject gameOverContainer;

    int currentHealth;
    int gameOverVirtualCameraPriority = 20;

    // ------------------- INITIALIZATION -------------------
    private void Awake()
    {
        currentHealth = startingHealth;
        AdjustShieldUI();
    }

    // ------------------- DAMAGE HANDLING -------------------
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        AdjustShieldUI();

        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    // ------------------- GAME OVER -------------------
    void PlayerGameOver()
    {
        weaponCamera.parent = null;
        deathVirtualCamera.Priority = gameOverVirtualCameraPriority;
        gameOverContainer.SetActive(true);

        MyPlayerInput myPlayerInput = FindFirstObjectByType<MyPlayerInput>();
        myPlayerInput.SetCursorState(false);

        Destroy(this.gameObject);
    }

    // ------------------- UI UPDATES -------------------
    void AdjustShieldUI()
    {
        for (int i = 0; i < shieldBars.Length; i++)
        {
            shieldBars[i].gameObject.SetActive(i < currentHealth);
        }
    }
}
