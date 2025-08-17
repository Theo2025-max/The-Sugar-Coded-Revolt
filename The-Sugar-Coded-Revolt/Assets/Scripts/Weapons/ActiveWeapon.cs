using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] WeaponSO weaponSO;
   
    Animator animator;
    MyPlayerInput myPlayerInput;
    Weapon currentWeapon;

    const string SHOOT_STRING = "Shoot";

    float timeSinceLastShoot = 0f;

    private void Awake()
    {
        myPlayerInput = GetComponentInParent<MyPlayerInput>();
        animator = GetComponent<Animator>();

    }

    private void Start()
    {
        currentWeapon = GetComponentInChildren<Weapon>();
    }
    void Update()
    {
        timeSinceLastShoot += Time.deltaTime;
        HandleShoot();
    }

    private void HandleShoot()
    {
        if (!myPlayerInput.shoot)  return;

        if (timeSinceLastShoot >= weaponSO.FireRate)
        {
           currentWeapon.Shoot(weaponSO);
            animator.Play(SHOOT_STRING, 0, 0f);
            timeSinceLastShoot = 0f;
        }

        myPlayerInput.ShootInput(false);


    }
}
