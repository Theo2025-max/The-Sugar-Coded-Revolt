using UnityEngine;

public class MyRigidBodyPush : MonoBehaviour
{
    // This lets me choose which layers the player can push
    public LayerMask pushableLayers;

    // I can turn pushing on or off with this toggle
    public bool canPush = true;

    // This controls how strong the push is
    [Range(0.5f, 5f)]
    public float pushStrength = 1.1f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If pushing is enabled, try to push what I collided with
        if (canPush)
        {
            TryPushRigidbody(hit);
        }
    }

    private void TryPushRigidbody(ControllerColliderHit hit)
    {
        // Grab the rigidbody from whatever I just hit
        Rigidbody body = hit.collider.attachedRigidbody;

        // If there's no rigidbody or it's not affected by physics, stop here
        if (body == null || body.isKinematic) return;

        // Make sure the object I hit is on a layer I'm allowed to push
        int collidedLayer = 1 << body.gameObject.layer;
        if ((collidedLayer & pushableLayers.value) == 0) return;

        // Ignore things below me (like standing on a box)
        if (hit.moveDirection.y < -0.3f) return;

        // Only push sideways, not up or down
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // Push the object using physics (like giving it a shove)
        body.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
    }
}
