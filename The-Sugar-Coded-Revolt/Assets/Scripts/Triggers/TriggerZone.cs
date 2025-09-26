using UnityEngine;
using TMPro;

public class TriggerZone : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject guidancePanel;
    [SerializeField] private TMP_Text instructionText;

    [Header("Controls")]
    [SerializeField] private KeyCode skipKey = KeyCode.Escape;
    [SerializeField] private bool deactivateAfterUse = true;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            guidancePanel.SetActive(true);
            instructionText.text = GetFullInstructionText();
            Time.timeScale = 0f; // freeze the game
        }
    }

    private void Update()
    {
        if (!isTriggered) return;

        if (Input.GetKeyDown(skipKey))
        {
            EndTutorial();
        }
    }

    private void EndTutorial()
    {
        guidancePanel.SetActive(false);
        Time.timeScale = 1f; // resume the game
        isTriggered = false;

        if (deactivateAfterUse)
            gameObject.SetActive(false);
    }

    private string GetFullInstructionText()
    {
        return
@"Movement
WASD or Arrow Keys – Move your character.
Left Shift – Sprint
Spacebar – Jump over obstacles.

Combat
Left Click – Shoot your weapon.
Right Click – Aim or zoom (for sniper rifles).
Hold Left Click – Continuous fire with automatic weapons.

Walk near a weapon to pick it up or switch guns
Walk into yellow glowing boxes to collect ammo

Win Condition: Destroy all enemies, enemy gates, and the vanguard gun!

Press ESC to close this tutorial.";
    }
}
