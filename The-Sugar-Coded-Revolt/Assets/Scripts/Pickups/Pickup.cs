using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    const string PLAYER_STRING = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_STRING))
        {
            ActiveWeapon activeWeapon = GetComponentInChildren<ActiveWeapon>();
            OnPickup(activeWeapon);
            Destroy(this.gameObject);   
        }
    }

    protected abstract void OnPickup(ActiveWeapon activeWeapon);
}
