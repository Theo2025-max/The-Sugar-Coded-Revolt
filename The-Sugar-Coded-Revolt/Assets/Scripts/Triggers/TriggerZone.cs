using UnityEngine;
using TMPro;

public class TriggerZone : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject guidancePanel;
    [SerializeField] private TMP_Text instructionText;

    [Header("Controls")]
    [SerializeField] private KeyCode skipKey = KeyCode.Backspace; // Default set to Backspace
    [SerializeField] private bool deactivateAfterUse = true;

    private bool isTriggered = false;

    private void Awake()
    {
        // Force the skip key to Backspace at runtime to override Inspector
        skipKey = KeyCode.Backspace;
    }

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
@"Movement:
WASD or Arrow Keys – Move
Left Shift – Sprint
Spacebar – Jump

Combat:
Left Click – Shoot
Right Click – Aim/Zoom
Hold Left Click – Automatic Fire

Interact:
Walk near a weapon to pick it up or switch guns
Walk into yellow glowing boxes to collect ammo

Objective:
Destroy all enemies, enemy gates, and the vanguard gun

Press Backspace to close this tutorial.";
    }
}
