using UnityEngine;


public class Weapon : MonoBehaviour
{
    [SerializeField] ParticleSystem muzzleFlash;
    public GameObject HitVFX;
    public void Shoot(WeaponSO weaponSO)
    {
        muzzleFlash.Play();
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {

            Instantiate(weaponSO.HitVFXPrefab, hit.point, Quaternion.identity);
            GameObject hitSparks = Instantiate(HitVFX, hit.point, Quaternion.identity);
            Destroy(hitSparks, 1 );
            EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
            enemyHealth?.TakeDamage(weaponSO.Damage);

        }
    }
}
