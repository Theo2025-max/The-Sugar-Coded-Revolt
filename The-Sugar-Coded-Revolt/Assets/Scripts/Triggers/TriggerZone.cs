using UnityEngine;
using UnityEngine.UI;

public class TriggerZone : MonoBehaviour
{
    public GameObject guidancePanel; // okay, this is my UI panel that pops up with controls
    private float displayDuration = 5f; // I'll show the panel for 5 seconds

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // only react if the thing entering is the Player
        {
            guidancePanel.SetActive(true); // show the panel when I walk in
            Invoke("HidePanel", displayDuration); // automatically hide it after a delay
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // again, make sure it's the Player leaving
        {
            HidePanel(); // hide the panel right away when I walk out
        }
    }

    private void HidePanel()
    {
        guidancePanel.SetActive(false); // just turn off the panel
    }
}

   