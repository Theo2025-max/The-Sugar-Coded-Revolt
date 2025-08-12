using UnityEditor.iOS;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject hitVFXPrefab;
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] private int damageAmount = 1;

    private MyPlayerInput myPlayerInput;

    const string SHOOT_STRING = "Shoot";

    private void Awake()
    {
        myPlayerInput = GetComponentInParent<MyPlayerInput>();
    }

    void Update()
    {
        HandleShoot();
    }

    private void HandleShoot()
    {
        if (!myPlayerInput.shoot)
            return;

        muzzleFlash.Play();
        animator.Play(SHOOT_STRING,0, 0f);
        myPlayerInput.ShootInput(false);


        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            Instantiate(hitVFXPrefab, hit.point, Quaternion.identity);
            
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            enemyHealth?.TakeDamage(damageAmount);

        }
    }
}
