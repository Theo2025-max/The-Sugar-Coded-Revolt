using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField] int startingHealth = 5;
    [SerializeField] CinemachineCamera deathVirtualCamera;
    [SerializeField] Transform weaponCamera;
    [SerializeField] Image[] shieldBars;
    [SerializeField] GameObject gameOverContainer;

    int currentHealth;
    int gameOverVirtualCameraPriority = 20;

    private void Awake()
    {
        currentHealth = startingHealth;
        AdjustShieldUI();
    }

    public void TakeDamage(int amount)
    {
        // Block damage if player has already won
        if (GameManager.instance != null && GameManager.instance.HasPlayerWon())
            return;

        currentHealth -= amount;
        AdjustShieldUI();

        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    void PlayerGameOver()
    {
        weaponCamera.parent = null;
        deathVirtualCamera.Priority = gameOverVirtualCameraPriority;
        gameOverContainer.SetActive(true);

        MyPlayerInput myPlayerInput = FindFirstObjectByType<MyPlayerInput>();
        myPlayerInput.SetCursorState(false);

        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerDied();
        }

        Destroy(this.gameObject);
    }

    void AdjustShieldUI()
    {
        for (int i = 0; i < shieldBars.Length; i++)
        {
            shieldBars[i].gameObject.SetActive(i < currentHealth);
        }
    }

    // ------------------- WIN REFILL LOGIC -------------------
    public void HealToFull(float delayBeforeStart = 0.5f, float durationPerShield = 0.2f)
    {
        StopAllCoroutines(); // Stop any ongoing animations
        StartCoroutine(AnimateShieldsRefill(delayBeforeStart, durationPerShield));
    }

    private IEnumerator AnimateShieldsRefill(float delayBeforeStart, float durationPerShield)
    {
        // Wait a short delay before starting refill (for cinematic effect)
        yield return new WaitForSeconds(delayBeforeStart);

        for (int i = currentHealth; i < startingHealth; i++)
        {
            shieldBars[i].gameObject.SetActive(true);
            currentHealth = i + 1;
            yield return new WaitForSeconds(durationPerShield);
        }
    }
}
